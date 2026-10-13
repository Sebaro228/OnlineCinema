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
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var res = await authService.LoginAsync(loginRequest);
            if (!res.success)
            {
                return BadRequest(res.message);
            }
            return Ok(res.response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterRequest register)
        {
            var res = await authService.RegisterAsync(register);
            if (!res.success)
            {
                return BadRequest(res.message);
            }
            return Ok(res.response);
        }
    }
}
