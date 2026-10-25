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
        public Task SaveTrailerDataAsync(Film f, Trailer t);
        public Task SaveTrailerFileAsync(Trailer T, Stream s);

        public Task<Stream> GetPosterAsync(Film f);
        public Task<Stream> GetVideoAsync(Film f);
        public Task<Trailer?> GetTrailerDataAsync(int trailerId);
        public Task<Stream> GetTrailerFileAsync(int trailerId);

        public Task DeletePosterAsync(Film f);
        public Task DeleteVideoAsync(Film f);
        public void DeleteTrailerDataAsync(Trailer t);
        public Task DeleteTrailerFileAsync(Trailer t);
        public Task DeleteAllTrailersAsync(Film f);



        public void UpdateTrailerData(Trailer t);
    }
}
