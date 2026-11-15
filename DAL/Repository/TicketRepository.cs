using DAL.Context;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class TicketRepository : Repository<Ticket>
    {
        public TicketRepository(CinemaContext context) : base(context) {}
    }
}