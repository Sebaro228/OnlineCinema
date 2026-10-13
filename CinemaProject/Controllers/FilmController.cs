using BLL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmController : ControllerBase
    {
        private readonly IService<Film> _service;

        public FilmController(IService<Film> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Stable");
        }
    }
}
