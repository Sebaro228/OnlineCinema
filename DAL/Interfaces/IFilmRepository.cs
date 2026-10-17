using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFilmRepository : IRepository<Film>
    {
        public Task SavePosterAsync(Film f, Stream s);
        public Task SaveVideoAsync(Film f, Stream s);
        public Task SaveTrailerAsync(Film f, Stream s);

        public Task<Stream> GetPosterAsync(Film f);
        public Task<Stream> GetVideoAsync(Film f);
        public Task<IEnumerable<Stream>> GetTrailersAsync(Film f);

        public Task DeletePosterAsync(Film f);
        public Task DeleteVideoAsync(Film f);
        public Task DeleteTrailerAsync(Film f, int id);
        public Task DeleteAllTrailersAsync(Film f);
    }
}
