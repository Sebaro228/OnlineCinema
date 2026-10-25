using BLL.Interfaces;
using DAL.Repository;
using Domain.Models;
using Domain.Models.DTO;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
namespace BLL.Services
{
    public class AuthService(UserRepository userRepository, JwtService service, RefreshTokenRepository refreshTokenRepository, UserManager<User> userManager)
    {
        public async Task<(bool success, string message, AuthResponse? response)> LoginAsync(LoginRequest login)
        {
            var user = await userRepository.GetByNameAsync(login.UserName);
            if (user == null) return (false, "User not found", null);
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash)) return (false, "Invalid password", null);
            var responce = await GetAuthResponceAsync(user);
            return (true, "Success", responce);
        }
        public async Task<(bool success, string message)> LogoutAsync(string token)
        {
            var tokenEntity = await refreshTokenRepository.GetRefreshAsync(token);
            if (tokenEntity == null) return (false, "Invalid refresh token");
            if (tokenEntity.isRevoked && !tokenEntity.IsActive) return (false, "Refresh token already revoked");
            await refreshTokenRepository.RevokeAllAsync(tokenEntity.UserId);
            await refreshTokenRepository.SaveChangeAsync();
            return (true, "Success");
        }
        public async Task<(bool success, string message, AuthResponse? response)> RefreshTokenAsync(string token)
        {
            var refreshTokenEntity = await refreshTokenRepository.GetRefreshAsync(token);
            if (refreshTokenEntity == null || refreshTokenEntity.IsExpired || !refreshTokenEntity.IsActive)
            {
                return (false, "Invalid refresh token", null);
            }
            refreshTokenEntity.isRevoked = true;
            refreshTokenRepository.Update(refreshTokenEntity);
            await refreshTokenRepository.SaveChangeAsync();
            var response = await GetAuthResponceAsync(refreshTokenEntity.User);
            return (true, "Token refreshed successfully", response);
        }
        public async Task<(bool success, string message, AuthResponse? response)> RegisterAsync(RegisterRequest register)
        {
            var user = await userRepository.GetByNameAsync(register.UserName);
            if (user != null) return (false, "User is already exist", null);
            user = await userRepository.GetByEmailAsync(register.Email);
            if (user != null) return (false, "User is already exist", null);
            user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password),
                CreatedAt = DateTime.UtcNow,
            };
            var res = await userManager.CreateAsync(user, register.Password);
            if (!res.Succeeded)
            {
                var errors = res.Errors.Select(x => x.Description);
                return (false, string.Join('\n', errors), null);
            }
            var responce = await GetAuthResponceAsync(user);
            return (true, "Success", responce);
        }
        async Task<AuthResponse> GetAuthResponceAsync(User user)
        {
            var jwttoken = service.GetJwt(user);
            var refreshToken = service.GetRefresh(user);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                Expired = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            await refreshTokenRepository.CreateAsync(refreshTokenEntity);
            await refreshTokenRepository.SaveChangeAsync();

            return new AuthResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                AccessToken = jwttoken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }
        public async Task<string?> GenereateTokenAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return null;
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
        public async Task<(bool success, string error)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return (false, "User not found");
            var res = await userManager.ResetPasswordAsync(user, token, newPassword);
            if (!res.Succeeded)
            {
                var error = res.Errors.Select(x => x.Description);
                return (false, string.Join(", ", error));
            }
            return (true, null);
        }
    }
}
