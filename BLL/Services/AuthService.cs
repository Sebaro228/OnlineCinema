using AutoMapper;
using AutoMapper.Execution;
using BCrypt.Net;
using BLL.Interfaces;
using DAL.Repository;
using Domain.DTOs;
using Domain.Models;
using Domain.Roles;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
namespace BLL.Services
{
    public class AuthService(JwtService service, RefreshTokenRepository refreshTokenRepository, UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService, IMapper mapper)
    {
        public async Task<(bool success, string message, AuthResponse? response)> LoginAsync(LoginRequest login)
        {
            var user = await userManager.FindByNameAsync(login.UserName);
            if (user == null) return (false, "User not found", null);
            var res = await signInManager.PasswordSignInAsync(user, login.Password, true, false);
            if (!res.Succeeded) return (false, "Invalid password", null);
            var responce = await GetAuthResponceAsync(user);
            return (true, "Success", responce);
        }
        public async Task<(bool success, string message, AuthResponse? response)> GoogleAuth(ProviderAuthRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.ProviderToken);
            var user = await userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User
                {
                    UserName = payload.Email.Split('@')[0],
                    Email = payload.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description);
                    return (false, string.Join(", ", errors), null);
                }
                await userManager.AddToRoleAsync(user, Roles.User);
            }
            await signInManager.SignInAsync(user, true);
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
            await signInManager.SignOutAsync();
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
        public async Task<(bool success, string message)> RegisterAsync(RegisterRequest register)
        {
            var user = await userManager.FindByNameAsync(register.UserName);
            if (user != null) return (false, "User is already exist");
            user = await userManager.FindByEmailAsync(register.Email);
            if (user != null) return (false, "User is already exist");
            user = mapper.Map<User>(register);
            var res = await userManager.CreateAsync(user, register.Password);
            if (!res.Succeeded)
            {
                var errors = res.Errors.Select(x => x.Description);
                return (false, string.Join('\n', errors));
            }
            var token = await GetConfirmTokenAsync(user.Id);
            var confirmationLink = $"https://cinema-frontend-black.vercel.app/confirm-email?email={Uri.EscapeDataString(register.Email)}&token={Uri.EscapeDataString(token)}";
            var msg = $"<p>Follow the link to confirm your email:</p>" +
                      $"<p><a href=\"{confirmationLink}\">Confirm Email</a></p>";
            await emailService.SendEmailAsync(register.Email, "Account confirmation", msg);
            await userManager.AddToRoleAsync(user, Roles.User);
            return (true, "Check your email");
        }
        async Task<AuthResponse> GetAuthResponceAsync(User user)
        {
            var jwttoken = await service.GetJwt(user);
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
                AccessToken = jwttoken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(15),
                Role = (await userManager.GetRolesAsync(user)).First()
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
            await refreshTokenRepository.RevokeAllAsync(user.Id);
            await refreshTokenRepository.SaveChangeAsync();
            return (true, null);
        }
        public async Task<string> GetConfirmTokenAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<(bool success, string error)> ConfirmEmailAsync(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return (false, "User not found");
            var res = await userManager.ConfirmEmailAsync(user, token);
            if (!res.Succeeded && token.Contains(' '))
            {
                res = await userManager.ConfirmEmailAsync(user, token.Replace(' ', '+'));
            }
            if (!res.Succeeded)
            {
                var error = res.Errors.Select(x => x.Description);
                return (false, string.Join(", ", error));
            }
            return (true, null);
        }
    }
}
