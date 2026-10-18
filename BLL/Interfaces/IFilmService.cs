using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFilmService : IService<Film>
    {
        public Task CratePosterAsync(Film f, Stream s);
        public Task CreateVideoAsync(Film f, Stream s);
        public Task CreateTrailerAsync(Film f, Stream s);

        public Task<Stream> GetPosterAsync(Film f);
        public Task<Stream> GetVideoAsync(Film f);
        public Task<IEnumerable<Stream>> GetTrailersAsync(Film f);

        public Task<(bool, string)> DeletePosterAsync(Film f);
        public Task<(bool, string)> DeleteVideoAsync(Film f);
        public Task<(bool, string)> DeleteTrailerAsync(Film f, int id);
        public Task<(bool, string)> DeleteAllTrailersAsync(Film f);

        public Task UpdatePosterAsync(Film f);
        public Task UpdateVideoAsync(Film f);
        public Task UpdateTrailerAsync(Film f, int id);
    }
}
