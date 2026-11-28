using BLL.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmSearchController : ControllerBase
    {
        private readonly IFilmSearchService _service;

        public FilmSearchController(IFilmSearchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<FilmSearchResultDto>> Search(
            [FromQuery] FilmSearchDto filters, CancellationToken cancellationToken)
        {
            if (filters is null)
                return BadRequest("Search parameters cannot be null.");

            if (filters.Title is not null && string.IsNullOrWhiteSpace(filters.Title))
                return BadRequest("Title cannot contain only whitespace.");

            if (filters.Description is not null && string.IsNullOrWhiteSpace(filters.Description))
                return BadRequest("Description cannot contain only whitespace.");

            if (filters.Year.HasValue &&
                (filters.Year.Value < 1880 || filters.Year.Value > DateTime.UtcNow.Year))
                return BadRequest("Year must be between 1880 and the current year.");

            if (filters.Page < 1)
                return BadRequest("Page must be greater than zero.");

            if (filters.PageSize < 1 || filters.PageSize > 100)
                return BadRequest("PageSize must be between 1 and 100.");

            if ((long)(filters.Page - 1) * filters.PageSize > int.MaxValue)
                return BadRequest("Requested page is too large.");

            FilmSearchResultDto result = await _service.SearchAsync(filters, cancellationToken);
            return Ok(result);
        }
    }
}
