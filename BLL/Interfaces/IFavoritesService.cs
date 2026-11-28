using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFavoritesService
    {
        public Task AddFavoriteFilmAsync(User u, Film f);
        public Task RemoveFavoriteFilmAsync(User u, Film f);
        public List<Film> GetFavoritesAsync(User u);
    }
}
