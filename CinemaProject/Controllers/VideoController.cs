using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private static readonly Dictionary<string, string[]> AllowedVideoTypes = new()
        {
            ["video/mp4"] = [".mp4"],
            ["video/webm"] = [".webm"],
            ["video/ogg"] = [".ogg", ".ogv"],
            ["video/quicktime"] = [".mov"],
            ["video/x-msvideo"] = [".avi"],
            ["video/x-matroska"] = [".mkv"],
            ["video/mpeg"] = [".mpeg", ".mpg"],
            ["video/3gpp"] = [".3gp"]
        };

        private readonly FilmService _service;
        public VideoController(FilmService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateVideo(int id, IFormFile video)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (!string.IsNullOrEmpty(f.VideoExtension))
                return BadRequest("This film already has a video");

            if (video is null)
                return BadRequest("The video is null");
            if (video.Length <= 0)
                return BadRequest("The file size is 0");


            string mime = video.ContentType;
            string ext = Path.GetExtension(video.FileName);

            if (!AllowedVideoTypes.Values.Any(e =>
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedVideoTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedVideoTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            f.VideoExtension = Path.GetExtension(video.FileName);
            f.VideoMime = mime;

            await _service.UpdateAsync(f);
            await _service.CreateVideoAsync(f, video.OpenReadStream());
            return Created();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");
            if (string.IsNullOrEmpty(f.VideoExtension))
                return BadRequest("This film has no video");

            await _service.DeleteVideoAsync(f);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetVideo(int id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (string.IsNullOrEmpty(f.VideoExtension))
                return NotFound("A film has no video");
            Stream s = await _service.GetVideoAsync(f);
            return File(s, f.VideoMime);
        }

        [HttpPut("UpdateVideo")]
        public async Task<IActionResult> UpdateVideo(int id, IFormFile video)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound("A film has not been found");

            if (video is null)
                return BadRequest("The video is null");
            if (video.Length <= 0)
                return BadRequest("The file size is 0");

            string mime = video.ContentType;
            string ext = Path.GetExtension(video.FileName);

            if (!AllowedVideoTypes.Values.Any(e =>
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedVideoTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedVideoTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            await _service.UpdateVideoAsync(f, video.OpenReadStream(),
                ext, mime);
            return Ok();
        }

    }
}
