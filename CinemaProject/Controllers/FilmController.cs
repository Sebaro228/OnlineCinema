using BLL.Interfaces;
using BLL.Services;
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

        public FilmController(FilmService service)
        {
            _service = service;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Stable");
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Film f)
        {
            await _service.CreateAsync(f);
            return Ok("Stable");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Film f) //Написать отдельный сервис, или расширить интерфейс создав новый, унаследовав его от текущего
        {
            await _service.DeleteAsync(0);
            return Ok("Stable");
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
    }
}
