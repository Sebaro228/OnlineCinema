using BLL.Services;
using Domain.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class UserController(UserService service) : ControllerBase
    {
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(string id, string role)
        {
            var (success, message) = await service.AddRole(id, role);
            if (!success) return BadRequest(message);
            return Ok();
        }
        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole(string id, string role)
        {
            var (success, message) = await service.RemoveRole(id, role);
            if (!success) return BadRequest(message);
            return Ok();
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await service.GetAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            await service.DeleteAsync(id);
            return Ok();
        }
    }
}
