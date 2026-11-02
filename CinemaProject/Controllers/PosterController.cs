using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PosterController : ControllerBase
    {
        private static readonly Dictionary<string, string[]> AllowedPosterTypes = new()
        {
            ["image/jpeg"] = [".jpg", ".jpeg"],
            ["image/png"] = [".png"],
            ["image/webp"] = [".webp"]
        };

        private readonly IFilmService _service;

        public PosterController(IFilmService service)
        {
            _service = service;
        }


        [HttpPost("Create")]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> CreatePoster(string id, IFormFile poster)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (!string.IsNullOrEmpty(f.PosterExtension))
                return BadRequest("This film already has a poster");

            if (poster is null)
                return BadRequest("The poster is null");
            if (poster.Length <= 0)
                return BadRequest("The file size is 0");


            string mime = poster.ContentType;
            string ext = Path.GetExtension(poster.FileName);

            if (!AllowedPosterTypes.Values.Any(e => 
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedPosterTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedPosterTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            await _service.UpdatePosterAsync(f, poster.OpenReadStream(), ext, mime);
            return Created();
        }

        [HttpDelete("Delete")]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> DeletePoster(string id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");
            if (string.IsNullOrEmpty(f.PosterExtension))
                return NotFound("This film has no poster");

            await _service.DeletePosterAsync(f);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetPoster(string id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (string.IsNullOrEmpty(f.PosterExtension))
                return NotFound("This film has no poster");
            Stream s = await _service.GetPosterAsync(f);
            return File(s, f.PosterMime);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> UpdatePoster(string id, IFormFile poster)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (poster is null)
                return BadRequest("The poster is null");
            if (poster.Length <= 0)
                return BadRequest("The file size is 0");

            string mime = poster.ContentType;
            string ext = Path.GetExtension(poster.FileName);

            if (!AllowedPosterTypes.Values.Any(e =>
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedPosterTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedPosterTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            await _service.UpdatePosterAsync(f, poster.OpenReadStream(),
                ext, mime);

            return Ok();
        }

    }
}
