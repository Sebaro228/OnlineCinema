using DAL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : Service<User>
    {
        static UserManager<User> _userManager;
        static RoleManager<IdentityRole> _roleManager;
        public UserService(IRepository<User> rep, UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : base(rep)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task AddRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return;
            if (await _roleManager.RoleExistsAsync(role)) await _userManager.AddToRoleAsync(user, role);
        }
        public async Task RemoveRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return;
            if (await _roleManager.RoleExistsAsync(role)) await _userManager.RemoveFromRoleAsync(user, role);
        }
    }
}
