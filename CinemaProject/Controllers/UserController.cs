using BLL.Services;
using Domain.Models;
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
        [HttpPost("ChangeRole")]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            await service.ChangeRole(id, role);
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
            var user = await service.GetAsync(id);
            if (user is null)
                return NotFound();

            await service.DeleteAsync(user);
            return Ok();
        }
    }
}
