using Azure.Storage.Blobs;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repository;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FilmService : Service<Film>, IFilmService
    {
        private readonly IFilmRepository _repository;

        public FilmService(IFilmRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public override async Task DeleteAsync(int id)
        {
            Film? f = await _repository.GetAsync(id);
            if (f is null)
                return;

            if (!string.IsNullOrEmpty(f.PosterExtension))
                await DeletePosterAsync(f);
            if (f.Trailers.Any())
                await DeleteAllTrailersAsync(f);
            if (!string.IsNullOrEmpty(f.VideoExtension))
                await DeleteVideoAsync(f);

            await _repository.DeleteAsync(f);
            await _repository.SaveChangeAsync();
        }

        public async Task CratePosterAsync(Film f, Stream s)
        {
            await _repository.SavePosterAsync(f, s);
        }

        public async Task CreateTrailerDataAsync(Film f, Trailer t)
        {
            await _repository.SaveTrailerDataAsync(f, t);
        }

        public async Task CreateTrailerFileAsync(Trailer t, Stream s)
        {
            await _repository.SaveTrailerFileAsync(t, s);
        }

        public async Task CreateVideoAsync(Film f, Stream s)
        {
            await _repository.SaveVideoAsync(f, s);
        }



        public async Task DeletePosterAsync(Film f)
        {
            await _repository.DeletePosterAsync(f);
            f.PosterExtension = string.Empty;
            f.PosterMime = string.Empty;
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteTrailerDataAsync(int id)
        {
            Trailer? t = await _repository.GetTrailerDataAsync(id);
            if(t is null)
                return;
            _repository.DeleteTrailerDataAsync(t);
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteTrailerFileAsync(int id)
        {
            Trailer? t = await _repository.GetTrailerDataAsync(id);
            if (t is null)
                return;
            await _repository.DeleteTrailerFileAsync(t);
            t.Extension = string.Empty;
            t.Mime = string.Empty;
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteAllTrailersAsync(Film f)
        {
            foreach (Trailer t in f.Trailers)
            {
                await _repository.DeleteTrailerFileAsync(t);
                _repository.DeleteTrailerDataAsync(t);
            }
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteVideoAsync(Film f)
        {
            await _repository.DeleteVideoAsync(f);
            f.VideoExtension = string.Empty;
            f.PosterExtension = string.Empty;
            await _repository.SaveChangeAsync();
        }



        public async Task<Stream> GetPosterAsync(Film f)
        {
            return await _repository.GetPosterAsync(f);
        }

        public async Task<Trailer?> GetTrailerDataAsync(int id)
        {
            return await _repository.GetTrailerDataAsync(id);
        }

        public async Task<Stream> GetTrailerFileAsync(int id)
        {
            Trailer? t = await _repository.GetTrailerDataAsync(id);
            if (t is null)
                return Stream.Null;
            return await _repository.GetTrailerFileAsync(t);
        }

        public async Task<Stream> GetVideoAsync(Film f)
        {
            return await _repository.GetVideoAsync(f);
        }



        public async Task UpdatePosterAsync(Film f, Stream s, string newExt, string newMime)
        {
            await _repository.DeletePosterAsync(f);
            f.PosterMime = newMime;
            f.PosterExtension = newExt;
            _repository.Update(f);
            await _repository.SavePosterAsync(f, s);
        }

        public async Task UpdateTrailerDataAsync(Trailer t)
        {
            _repository.UpdateTrailerData(t);
            await _repository.SaveChangeAsync();
        }

        public async Task UpdateTrailerFileAsync(Trailer t, Stream s, string newExt, string newMime)
        {
            await _repository.DeleteTrailerFileAsync(t);
            t.Extension = newExt;
            t.Mime = newMime;
            _repository.UpdateTrailerData(t);
            await _repository.SaveTrailerFileAsync(t, s);
        }

        public async Task UpdateVideoAsync(Film f, Stream s, string newExt, string newMime)
        {
            await _repository.DeleteVideoAsync(f);
            f.VideoMime = newMime;
            f.VideoExtension = newExt;
            _repository.Update(f);
            await _repository.SaveVideoAsync(f, s);
        }
    }
}
