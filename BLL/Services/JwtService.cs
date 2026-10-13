using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class JwtService(IConfiguration configuration)
    {
        public string GetJwt(User user)
        {
            var key = configuration["Key"];
            var issuer = configuration["Jwt:Issuer "];
            var ausience = configuration["Jwt:Audience"];
            var exp = int.Parse(configuration["Jwt:ExpireMinutes"]);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {       
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: ausience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );
            var result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }

        public string GetRefresh(User user)
        {
            var rand = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(rand);
                return Convert.ToBase64String(rand);
            }
        }
    }
}
