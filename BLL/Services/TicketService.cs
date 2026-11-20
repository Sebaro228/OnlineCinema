using DAL.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class TicketService : Service<Ticket> 
    {
        IRepository<Ticket> _rep;
        public TicketService(IRepository<Ticket> rep) : base(rep)
        {
            _rep = rep;
        }

        public async Task CreateTicket(CreateTicket request, string userId)
        {
            var ticket = new Ticket
            {
                Topic = request.Topic,
                UserId = userId,
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
            return (true, "Ticket accepted successfully");
        }
    }
}
