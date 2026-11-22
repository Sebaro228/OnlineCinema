using BLL.Hubs;
using DAL.Interfaces;
using Domain.DTOs;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class TicketService : Service<Ticket> 
    {
        IRepository<Ticket> _rep;
        EmailService _emailService;
        UserManager<User> _userManager;
        IConfiguration _configuration;
        IResend _resendClient;
        ChatHub _chatHub;
        public TicketService(IRepository<Ticket> rep, EmailService emailService, UserManager<User> userManager, IConfiguration configuration, IResend resendClient, ChatHub chatHub) : base(rep)
        {
            _rep = rep;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _resendClient = resendClient;
            _chatHub = chatHub; 
        }

        public async Task CreateTicket(CreateTicket request, string userId)
        {
            var ticket = new Ticket
            {
                Topic = request.Topic,
                UserId = userId,
                ConversationType = request.ConversationType
            };
            ticket.Messages.Add(new Message
            {
                Content = request.FirstMessage,
                FilePath = request.FilePath,
                SenderId = userId
            });
            if (ticket.ConversationType == Conversation.LiveChat)
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _chatHub.JoinChat(ticket.Id, user.UserName);
                await _chatHub.SendMessageToChat(ticket.Id, user.UserName, new MessageDto
                {
                    Content = request.FirstMessage,
                    FilePath = request.FilePath
                });
            }
            await CreateAsync(ticket);
        }
        public async Task<(bool success, string? message)> AcceptTicket(string ticketId, string agentId, MessageDto message)
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            if (!ticket.IsOpen) return (false, "Ticket is closed");
            if (!string.IsNullOrEmpty(ticket.AgentId)) return (false, "Ticket is already accepted by another agent");
            ticket.AgentId = agentId;
            await UpdateAsync(ticket);
            if (ticket.ConversationType == Conversation.LiveChat)
            {
                var user = await _userManager.FindByIdAsync(agentId);
                await _chatHub.JoinChat(ticket.Id, user.UserName);
            }
            var responce = await Responce(ticketId, agentId, message);
            return responce;
        }
        public async Task<(bool success, string? message)> Responce(string ticketId, string id, MessageDto message) 
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            if (!ticket.IsOpen) return (false, "Ticket is closed");
            if (ticket.AgentId != id && ticket.UserId != id) return (false, "You are not the owner of this ticket");
            ticket.Messages.Add(new Message
            {
                Content = message.Content,
                FilePath = message.FilePath,
                SenderId = id
            });
            await UpdateAsync(ticket);
            if (ticket.ConversationType == Conversation.Email)
            {
                var user = await _userManager.FindByIdAsync(ticket.UserId);
                var from = _configuration["Support:FromEmail"];
                var name = _configuration["Support:FromName"];
                await _emailService.SendEmailAsync(user.Email, ticket.Topic, message.Content, from, name, message.FilePath);
            }
            if (ticket.ConversationType == Conversation.LiveChat)
            {
                if (ticket.AgentId == id)
                {
                    var agent = await _userManager.FindByIdAsync(ticket.AgentId);
                    await _chatHub.SendMessageToChat(ticket.Id, agent.UserName, message);
                }
                else if (ticket.UserId == id)
                {
                    var user = await _userManager.FindByIdAsync(ticket.UserId);
                    await _chatHub.SendMessageToChat(ticket.Id, user.UserName, message);
                }
            }
            return (true, null);
        }
        public async Task<(bool success, string? message)> CloseTicket(string ticketId, string id)
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            if (!ticket.IsOpen) return (false, "Ticket is already closed");
            if (ticket.AgentId != id && ticket.UserId != id) return (false, "You are not the owner of this ticket");
            ticket.IsOpen = false;
            ticket.ClosedAt = DateTime.UtcNow;
            await UpdateAsync(ticket);
            if (ticket.ConversationType == Conversation.LiveChat)
            {
                var user = await _userManager.FindByIdAsync(ticket.UserId);
                var agent = await _userManager.FindByIdAsync(ticket.AgentId);
                _chatHub.LeaveChat(ticket.Id, user.UserName);
                _chatHub.LeaveChat(ticket.Id, agent.UserName);
            }
            return (true, null);
        }
        public async Task<(bool success, string? message)> HandleEmailReceived(ResendWebhookPayload payload) 
        {
            if (payload.Type != "email.received" || payload.Data == null)   
                return (false, "Invalid payload");
            var emailResponse = await _resendClient.ReceivedEmailRetrieveAsync(payload.Data.EmailId);
            var email = emailResponse.Content;
            if (email == null) return (false, "Email not found");
            var user = await _userManager.FindByEmailAsync(email.From.Email);
            if (user == null) return (false, "User not found");
            var ticket = (await GetAllAsync()).FirstOrDefault(t => t.UserId == user.Id && t.IsOpen);
            if (ticket != null)
            {
                string emailIdStr = email.Id.ToString();
                if (!ticket.Messages.Any(m => m.Id == emailIdStr))
                {
                    ticket.Messages.Add(new Message
                    {
                        Id = emailIdStr,
                        TicketId = ticket.Id,
                        SenderId = user.Id,
                        Content = email.TextBody ?? email.HtmlBody ?? string.Empty,
                        SendedAt = email.MomentCreated ?? DateTime.UtcNow,
                        FilePath = email.Attachments?.FirstOrDefault()?.Filename
                    });
                    await UpdateAsync(ticket);
                }
            }
            return (true, "Email processed successfully");
        }
    }
}
