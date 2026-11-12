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
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class FilmController : ControllerBase
    {
        private readonly IFilmService _service;
        private readonly IMapper _mapper;

        public FilmController(IFilmService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> CreateFilm([FromBody] FilmDto dto)
        {
            if(string.IsNullOrEmpty(dto.Title))
                return BadRequest("A title can not be empty");
            if(string.IsNullOrEmpty(dto.Description))
                return BadRequest("A description can not be empty");
            if (dto.Year < 1880 || dto.Year > DateTime.Now.Year)
                return BadRequest("A year can not be below 1880 and not above current year");

            Film f = _mapper.Map<Film>(dto);
            await _service.CreateAsync(f);
            return Ok(f);
        }

        [HttpDelete("Delete")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> Delete(string id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound();

            await _service.DeleteAsync(f);
            return Ok("Deleted");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(string id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound();
            return Ok(f);
        }

        [HttpPut("Update")]
        //[Authorize(Roles = "ContentManager")]
        public async Task<IActionResult> Update(string id, FilmDto dto)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound();

            if (dto.Year != 0)
            {
                if (dto.Year < 1880 || dto.Year > DateTime.Now.Year)
                    return BadRequest("A year can not be below 1880 and not above current year");
                f.Year = dto.Year;
            }
            if (!string.IsNullOrEmpty(dto.Title))
                f.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description))
                f.Description = dto.Description;
            
            await _service.UpdateAsync(f);
            return Ok();
        }
    }
}
