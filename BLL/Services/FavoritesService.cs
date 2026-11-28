using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IRepository<User> _userRepository;

        public FavoritesService(IRepository<User> repository)
        {
            _userRepository = repository;
        }

        public async Task AddFavoriteFilmAsync(User u, Film f)
        {
            u.Favorites.Add(f);
            await _userRepository.SaveChangeAsync();
        }

        public async Task RemoveFavoriteFilmAsync(User u, Film f)
        {
            u.Favorites.Remove(f);
            await _userRepository.SaveChangeAsync();
        }

        public List<Film> GetFavoritesAsync(User u)
            => u.Favorites;
    }
}
