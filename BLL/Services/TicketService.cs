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
        UserManager<User> _userManager;
        public TicketService(IRepository<Ticket> rep, UserManager<User> userManager) : base(rep)
        {
            _rep = rep;
            _userManager = userManager;
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
                FilePath = request.FilePath
            });
            await _rep.CreateAsync(ticket);
        }
    }
}
