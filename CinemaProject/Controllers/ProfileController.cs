using AutoMapper;
using BLL.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController(ProfileService service, IMapper mapper) : ControllerBase
    {
        string userId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = await service.GetProfileAsync(userId);
            return Ok(profile);
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfile updateProfileDTO)
        {
            await service.UpdateProfileAsync(userId, updateProfileDTO);
            return Ok();
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword changePasswordDTO)
        {
            var (success, error) = await service.ChangePassword(userId, changePasswordDTO);
            if (!success) return BadRequest(error);
            return Ok();
        }
        [HttpDelete("DeleteProfile")]
        public async Task<IActionResult> DeleteProfile(DeleteProfile deleteProfileDTO)
        {
            var (success, error) = await service.DeleteProfile(userId, deleteProfileDTO);
            if (!success) return BadRequest(error);
            return Ok();
        }
    }
}