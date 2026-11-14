using DAL.Context;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class ParticipantRepository : Repository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(CinemaContext ctx) : base(ctx) { }

        public override async Task<IEnumerable<Participant>> GetAllAsync()
            => await _context.Participants
                .Include(p => p.Participations)
                    .ThenInclude(fp => fp.Film)
                .ToListAsync();

        public override async Task<Participant?> GetAsync(string id)
            => await _context.Participants
                .Include(p => p.Participations)
                    .ThenInclude(fp => fp.Film)
                .FirstOrDefaultAsync(p => string.Equals(p.Id, id));
    }
}
