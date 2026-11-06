using Domain.Models;
using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Seeder
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(Roles.Admin)) await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            if (!await roleManager.RoleExistsAsync(Roles.User)) await roleManager.CreateAsync(new IdentityRole(Roles.User));
            if (!await roleManager.RoleExistsAsync(Roles.ContentManager)) await roleManager.CreateAsync(new IdentityRole(Roles.ContentManager));
            if (!await roleManager.RoleExistsAsync(Roles.ContentReviewer)) await roleManager.CreateAsync(new IdentityRole(Roles.ContentReviewer));
            if (!await roleManager.RoleExistsAsync(Roles.SupportAgent)) await roleManager.CreateAsync(new IdentityRole(Roles.SupportAgent));
        }
        public static async Task SeedAdminUserAsync(IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var email = "admin@coolcinema.me";
            var name = "cinema";
            var password = "Admin228@";
            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new User
                {
                    Email = email,
                    UserName = name,
                    EmailConfirmed = true
                };
                var res = await userManager.CreateAsync(admin, password);
                if (res.Succeeded) await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
