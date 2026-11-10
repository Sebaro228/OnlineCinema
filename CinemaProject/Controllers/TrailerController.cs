using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TrailerController : ControllerBase
    {
        private static readonly Dictionary<string, string[]> AllowedTrailerTypes = new()
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



        private readonly IFilmService _service;
        private readonly IMapper _mapper;
        public TrailerController(IFilmService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> CreateTrailer(TrailerDto dto)
        {
            Film? f = await _service.GetAsync(dto.FilmId);
            if (f is null)
                return NotFound("The film was not found");

            Trailer t = _mapper.Map<Trailer>(dto);

            await _service.CreateTrailerDataAsync(f, t);
            return Created();
        }

        [HttpPost("CreateFile")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> CreateTrailerFile(string trailerId, IFormFile trailer)
        {
            if (trailer is null)
                return BadRequest("The trailer is null");
            if (trailer.Length <= 0)
                return BadRequest("The file size is 0");

            string mime = trailer.ContentType;
            string ext = Path.GetExtension(trailer.FileName);

            if (!AllowedTrailerTypes.Values.Any(e =>
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedTrailerTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedTrailerTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            Trailer? t = await _service.GetTrailerDataAsync(trailerId);
            if (t is null)
                return NotFound("The trailer was not found");

            await _service.UpdateTrailerFileAsync(t, trailer.OpenReadStream(), ext, mime);
            return Created();
        }

        [HttpDelete("Delete")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> DeleteTrailerData(string trailerId)
        {
            Trailer? t = await _service.GetTrailerDataAsync(trailerId);
            if (t is null)
                return NotFound("The trailer has not been found");
            
            await _service.DeleteTrailerFileAsync(trailerId);
            await _service.DeleteTrailerDataAsync(trailerId);
            return Ok();
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetTrailer(string trailerId)
        {
            Trailer? t = await _service.GetTrailerDataAsync(trailerId);
            if (t is null)
                return NotFound("The trailer has not been found");

            return Ok(t);
        }

        [HttpGet("GetFile")]
        public async Task<IActionResult> GetTrailerStream(string trailerId)
        {
            Trailer? t = await _service.GetTrailerDataAsync(trailerId);
            if (t is null)
                return NotFound("The trailer has not been found");
            if (string.IsNullOrEmpty(t.Extension))
                return NotFound("This trailer has no file");
            return File(await _service.GetTrailerFileAsync(trailerId), t.Mime);
        }

        [HttpGet("GetAllTrailersData")]
        public async Task<IActionResult> GetAllTrailersData(string filmId)
        {
            Film? f = await _service.GetAsync(filmId);
            if (f is null)
                return NotFound("The film was not found");

            return Ok(f.Trailers);
        }

        [HttpPut("UpdateData")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> UpdateTrailerData(string id, TrailerDto dto)
        {
            Trailer? t = await _service.GetTrailerDataAsync(id);
            if (t is null)
                return NotFound("The trailer has not been found");

            if (!string.IsNullOrEmpty(dto.FilmId))
            {
                Film? newFilm = await _service.GetAsync(dto.FilmId);
                if (newFilm is null)
                    return NotFound("A new film has not been found");

                t.FilmId = dto.FilmId;
                t.Film = newFilm;
            }

            if(!string.IsNullOrEmpty(dto.Title))
                t.Title = dto.Title;
            if(dto.Date != default && dto.Date < DateOnly.FromDateTime(DateTime.UtcNow))
                t.Date = dto.Date;
            await _service.UpdateTrailerDataAsync(t);
            return Ok();
        }

        [HttpPut("UpdateFile")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> UpdateTrailerFile(string trailerId, IFormFile trailer)
        {
            Trailer? t = await _service.GetTrailerDataAsync(trailerId);
            if (t is null)
                return NotFound("A trailer has not been found");

            if (trailer is null)
                return BadRequest("The trailer is null");
            if (trailer.Length <= 0)
                return BadRequest("The trailer size is 0");

            string mime = trailer.ContentType;
            string ext = Path.GetExtension(trailer.FileName);

            if (!AllowedTrailerTypes.Values.Any(e =>
                e.Any(s => string.Equals(s, ext))))
                return BadRequest("Unallowed extension");

            if (!AllowedTrailerTypes.Keys.Any(m => string.Equals(m, mime)))
                return BadRequest("Unallowed mime");

            if (!AllowedTrailerTypes[mime].Any(e => string.Equals(e, ext)))
                return BadRequest("Mime does not match the specified extension of the file");

            await _service.UpdateTrailerFileAsync(t, trailer.OpenReadStream(),
                ext, mime);

            return Ok();
        }
    }
}
