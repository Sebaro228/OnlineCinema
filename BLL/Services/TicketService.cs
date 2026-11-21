using DAL.Interfaces;
using Domain.DTOs;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        public TicketService(IRepository<Ticket> rep, EmailService emailService, UserManager<User> userManager, IConfiguration configuration) : base(rep)
        {
            _rep = rep;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
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
        public async Task<(bool success, string message)> AcceptTicket(string ticketId, string agentId, MessageDto message)
        {
            var ticket = await GetAsync(ticketId);
            if (ticket == null) return (false, "Ticket not found");
            ticket.AgentId = agentId;
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
            return (true, "Ticket accepted successfully");
        }
    }
}
