using BLL.Interfaces;
using BLL.Services;
using Domain.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(res.response);
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
            var msg = "Copy your token for resetting";
            await emailService.SendEmailAsync(model.Email, "Password reset", msg);
            return Ok("Check your email");
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            var res = await authService.ResetPasswordAsync(model.Email, model.Token, model.Password);
            if (!res.success) return BadRequest(res.error);
            return RedirectToAction("Login");
        }
    }
}
