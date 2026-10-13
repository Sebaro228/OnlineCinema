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
namespace BLL.Services
{
    public class AuthService(UserRepository userRepository, JwtService service, RefreshTokenRepository refreshTokenRepository)
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
            if (refreshTokenEntity == null && refreshTokenEntity.IsExpired && !refreshTokenEntity.IsActive)
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
                Name = register.UserName,
                Email = register.Email,
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password),
                CreatedAt = DateTime.UtcNow,
            };
            await userRepository.CreateAsync(user);
            await userRepository.SaveChangeAsync();
            return (true, "Success", new AuthResponse
            {
                UserName = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessToken = service.GetJwt(user),
                RefreshToken = service.GetRefresh(user),
                Expiration = DateTime.UtcNow.AddMinutes(15)
            });
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
                UserName = user.Name,
                Email = user.Email,
                Role = user.Role,
                AccessToken = jwttoken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };
        }
    }
}
