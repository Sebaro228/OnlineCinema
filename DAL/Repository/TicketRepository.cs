using DAL.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class TicketRepository : Repository<Ticket>
    {
        public TicketRepository(CinemaContext context) : base(context) {}

        public override async Task<IEnumerable<Ticket>> GetAllAsync()
            => await _dbSet.Include(t => t.Messages).ToListAsync();

        public override async Task<Ticket?> GetAsync(string id)
            => await _dbSet.Include(t => t.Messages).FirstOrDefaultAsync(t => t.Id == id);

        public override async Task<Ticket?> FirstOrDefaultAsync(Expression<Func<Ticket, bool>> pred)
            => await _dbSet.Include(t => t.Messages).FirstOrDefaultAsync(pred);
    }
}