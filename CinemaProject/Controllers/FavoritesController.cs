using BLL.Interfaces;
using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IFilmService _filmService;
        private readonly IFavoritesService _favoritesService;
        private readonly IService<User> _userService;

        public FavoritesController(
            UserManager<User> userManager,
            IFilmService filmService,
            IFavoritesService favoritesService,
            IService<User> userService
        )
        {
            _userManager = userManager;
            _filmService = filmService;
            _favoritesService = favoritesService;
            _userService = userService;
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<IActionResult> AddFavoriteFilm(string filmId)
        {
            User? u = await _userManager.GetUserAsync(User);
            if (u is null)
                return Unauthorized();

            Film? f = await _filmService.GetAsync(filmId);
            if( f is null)
                return NotFound("A film was not found");

            await _favoritesService.AddFavoriteFilmAsync(u, f);
            return Ok();
        }

        [Authorize]
        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveFavoriteFilm(string filmId)
        {
            string? id = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id))
                return Unauthorized();

            User? u = await _userService.GetAsync(id);
            if(u is null)
                return Unauthorized();

            Film? f = await _filmService.GetAsync(filmId);
            if (f is null)
                return NotFound("A film was not found");

            if (!u.Favorites.Any(fav => string.Equals(f.Id, fav.Id)))
                return NotFound("This film is not in the favorites");

            await _favoritesService.RemoveFavoriteFilmAsync(u, f);
            return Ok();
        }

        [Authorize]
        [HttpPost("Get")]
        public async Task<IActionResult> GetFavorites()
        {
            string? id = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(id))
                return Unauthorized();

            User? u = await _userService.GetAsync(id);
            if(u is null)
                return Unauthorized();

            return Ok(u.Favorites);
        }
    }
}
