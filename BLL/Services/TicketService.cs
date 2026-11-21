using DAL.Interfaces;
using Domain.DTOs;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        public TicketService(IRepository<Ticket> rep, EmailService emailService, UserManager<User> userManager, IConfiguration configuration, IResend resendClient) : base(rep)
        {
            _rep = rep;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _resendClient = resendClient;
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
            var responce = await Responce(ticketId, agentId, message);
            return responce;
        }
        public async Task<(bool success, string? message)> Responce(string ticketId, string agentId, MessageDto message)
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            if (!ticket.IsOpen) return (false, "Ticket is closed");
            if (ticket.AgentId != agentId) return (false, "You are not the owner of this ticket");
            ticket.Messages.Add(new Message
            {
                Content = message.Content,
                FilePath = message.FilePath,
                SenderId = agentId
            });
            await UpdateAsync(ticket);
            var user = await _userManager.FindByIdAsync(ticket.UserId);
            if (ticket.ConversationType == Conversation.Email)
            {
                var from = _configuration["Support:FromEmail"];
                var name = _configuration["Support:FromName"];
                await _emailService.SendEmailAsync(user.Email, ticket.Topic, message.Content, from, name, message.FilePath);
            }
            return (true, null);
        }
        public async Task<(bool success, string? message)> CloseTicket(string ticketId, string agentId)
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            if (!ticket.IsOpen) return (false, "Ticket is already closed");
            if (ticket.AgentId != agentId) return (false, "You are not the owner of this ticket");
            ticket.IsOpen = false;
            ticket.ClosedAt = DateTime.UtcNow;
            await UpdateAsync(ticket);
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
