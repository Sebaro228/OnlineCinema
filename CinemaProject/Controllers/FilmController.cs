using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmController : ControllerBase
    {
        private readonly FilmService _service;
        private readonly IMapper _mapper;

        public FilmController(FilmService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }



        [HttpPost("Create")]
        public async Task<IActionResult> CreateFilm([FromBody] FilmDto dto)
        {
            Film f = _mapper.Map<Film>(dto);
            await _service.CreateAsync(f);
            return Created();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _service.FirstOrDefaultAsync(f => f.Id == id) is null)
                return NotFound();

            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound();
            return Ok(f);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, FilmDto dto)
        {
            Film? f = await _service.GetAsync(id);
            if (f is null)
                return NotFound();

            f.Title = dto.Title;
            f.Year = dto.Year;
            f.Description = dto.Description;
            
            await _service.UpdateAsync(f);
            return Ok();
        }
    }
}
