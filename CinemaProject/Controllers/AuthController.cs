using BLL.Interfaces;
using BLL.Services;
using Domain.DTOs;
using Domain.Enums;
using Domain.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService, EmailService emailService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var res = await authService.LoginAsync(loginRequest);
            if (!res.success)
            {
                return BadRequest(res.message);
            }
            return Ok(res.response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest register)
        {
            var res = await authService.RegisterAsync(register);
            if (!res.success)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
        [HttpPost("ProviderAuth")]
        public async Task<IActionResult> ProviderAuth(ProviderAuthRequest request)
        {
            if (request.ProviderType == Provider.Google)
            {
                var res = await authService.GoogleAuth(request);
                if (!res.success) return BadRequest(res.message);
                return Ok(res.response);
            }
            if (request.ProviderType == Provider.Apple)
            {
            
            }
            if (request.ProviderType == Provider.PhoneNumber)
            {

            }
            return Ok();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var res = await authService.LogoutAsync(refreshTokenRequest.RefreshToken);
            if (!res.success)
            {
                return BadRequest(res.message);
            }
            return Ok(res.message);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var res = await authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);
            if (!res.success)
            {
                return BadRequest(res.message);
            }

            return Ok(res.response);
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            var token = await authService.GenereateTokenAsync(model.Email);
            if (string.IsNullOrEmpty(token)) return BadRequest("Invalid email");
            var confirmationLink = $"https://cinema-frontend-black.vercel.app/reset-password?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";
            var msg = $"<p>Follow the link to reset your password:</p>" +
                      $"<p><a href=\"{confirmationLink}\">Reset Password</a></p>";
            await emailService.SendEmailAsync(model.Email, "Password reset", msg);
            return Ok("Check your email");
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            var res = await authService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            if (!res.success) return BadRequest(res.error);
            return Ok("Password was changed");
        }
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var res = await authService.ConfirmEmailAsync(email, token);
            if (!res.success) return BadRequest(res.error);
            return Ok("Account confirmed");
        }
    }
}
