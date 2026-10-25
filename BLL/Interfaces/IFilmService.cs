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
        public Task CreateTrailerDataAsync(Film f, Trailer t);
        public Task CreateTrailerFileAsync(Trailer t,  Stream s);

        public Task<Stream> GetPosterAsync(Film f);
        public Task<Stream> GetVideoAsync(Film f);
        public Task<Trailer?> GetTrailerDataAsync(int id);
        public Task<Stream> GetTrailerFileAsync(int id);

        public Task DeletePosterAsync(Film f);
        public Task DeleteVideoAsync(Film f);
        public Task DeleteTrailerDataAsync(Film f, int id);
        public Task DeleteTrailerFileAsync(Film f, int id);

        public Task UpdatePosterAsync(Film f, Stream s, string newExt, string newMime);
        public Task UpdateVideoAsync(Film f, Stream s, string newExt, string newMime);
        public Task UpdateTrailerDataAsync(Trailer t);
        public Task UpdateTrailerFileAsync(Trailer t, Stream s, string newExt, string newMime);
    }
}
