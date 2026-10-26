using DAL.Context;
using Domain.Models;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RefreshTokenRepository : Repository<RefreshToken>
    {
        public RefreshTokenRepository(CinemaContext context) : base(context)
        {
        }

        public async Task<RefreshToken> GetRefreshAsync(string token)
        {
            return await _dbSet.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task RevokeAllAsync(string id)
        {
            await _dbSet.Where(x => x.UserId == id).ForEachAsync(x => x.isRevoked = true);
        }
    }
}
