using DAL.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UserRepository : Repository<User>
    {   
        public UserRepository(CinemaContext context) : base(context) { }

        public override async Task<User?> GetAsync(string id)
            => await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => string.Equals(u.Id, id));



        public async Task<User?> GetByEmailAsync(string email) => await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        public async Task<User?> GetByNameAsync(string name) => await _dbSet.FirstOrDefaultAsync(x => x.UserName == name);
    }
}
