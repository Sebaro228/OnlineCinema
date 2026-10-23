using BLL.Services;
using Domain.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
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
    }
}
