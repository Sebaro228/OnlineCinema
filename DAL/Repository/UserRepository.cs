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
        public async Task<User?> GetByEmailAsync(string email) => await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        public async Task<User?> GetByNameAsync(string name) => await _dbSet.FirstOrDefaultAsync(x => x.UserName == name);
    }
}
