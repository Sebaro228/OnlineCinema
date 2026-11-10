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

        public override async Task DeleteAsync(string id)
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

        private async Task ReplaceFileAsync(string newPath, string? oldPath, Stream stream,
            Action applyMetadata, Action restoreMetadata)
        {
            try
            {
                await _repository.UploadBlobAsync(newPath, stream);
            }
            catch
            {
                await TryDeleteNewBlobAsync(newPath);
                throw;
            }

            try
            {
                applyMetadata();
                await _repository.SaveChangeAsync();
            }
            catch
            {
                restoreMetadata();
                await TryDeleteNewBlobAsync(newPath);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldPath))
            {
                try { await _repository.DeleteBlobAsync(oldPath); }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Old blob cleanup failed for '{oldPath}': {ex}");
                }
            }
        }

        private async Task TryDeleteNewBlobAsync(string path)
        {
            try { await _repository.DeleteBlobAsync(path); }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"New blob cleanup failed for '{path}': {ex}");
            }
        }

        public Task CratePosterAsync(Film f, Stream s)
            => UpdatePosterAsync(f, s, f.PosterExtension, f.PosterMime);

        public async Task CreateTrailerDataAsync(Film f, Trailer t)
        {
            await _repository.SaveTrailerDataAsync(f, t);
        }

        public Task CreateTrailerFileAsync(Trailer t, Stream s)
            => UpdateTrailerFileAsync(t, s, t.Extension, t.Mime);

        public Task CreateVideoAsync(Film f, Stream s)
            => UpdateVideoAsync(f, s, f.VideoExtension, f.VideoMime);


        public async Task DeletePosterAsync(Film f)
        {
            await _repository.DeletePosterAsync(f);
            f.PosterExtension = string.Empty;
            f.PosterMime = string.Empty;
            f.PosterBlobPath = string.Empty;
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteTrailerDataAsync(string id)
        {
            Trailer? t = await _repository.GetTrailerDataAsync(id);
            if(t is null)
                return;
            _repository.DeleteTrailerDataAsync(t);
            await _repository.SaveChangeAsync();
        }

        public async Task DeleteTrailerFileAsync(string id)
        {
            Trailer? t = await _repository.GetTrailerDataAsync(id);
            if (t is null)
                return;
            await _repository.DeleteTrailerFileAsync(t);
            t.Extension = string.Empty;
            t.Mime = string.Empty;
            t.BlobPath = string.Empty;
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
            f.VideoMime = string.Empty;
            f.VideoBlobPath = string.Empty;
            await _repository.SaveChangeAsync();
        }



        public async Task<Stream> GetPosterAsync(Film f)
        {
            return await _repository.GetPosterAsync(f);
        }

        public async Task<Trailer?> GetTrailerDataAsync(string id)
        {
            return await _repository.GetTrailerDataAsync(id);
        }

        public async Task<Stream> GetTrailerFileAsync(string id)
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
            string oldPath = f.PosterBlobPath;
            string oldExt = f.PosterExtension, oldMime = f.PosterMime, oldStoredPath = f.PosterBlobPath;
            string path = $"posters/{f.Id}/{Guid.NewGuid():N}{newExt}";
            await ReplaceFileAsync(path, oldPath, s,
                () => { f.PosterExtension = newExt; f.PosterMime = newMime; f.PosterBlobPath = path; },
                () => { f.PosterExtension = oldExt; f.PosterMime = oldMime; f.PosterBlobPath = oldStoredPath; });
        }

        public async Task UpdateTrailerDataAsync(Trailer t)
        {
            _repository.UpdateTrailerData(t);
            await _repository.SaveChangeAsync();
        }

        public async Task UpdateTrailerFileAsync(Trailer t, Stream s, string newExt, string newMime)
        {
            string oldPath = t.BlobPath;
            string oldExt = t.Extension, oldMime = t.Mime, oldStoredPath = t.BlobPath;
            string path = $"trailers/{t.FilmId}/{t.Id}/{Guid.NewGuid():N}{newExt}";
            await ReplaceFileAsync(path, oldPath, s,
                () => { t.Extension = newExt; t.Mime = newMime; t.BlobPath = path; },
                () => { t.Extension = oldExt; t.Mime = oldMime; t.BlobPath = oldStoredPath; });
        }

        public async Task UpdateVideoAsync(Film f, Stream s, string newExt, string newMime)
        {
            string oldPath = f.VideoBlobPath;
            string oldExt = f.VideoExtension, oldMime = f.VideoMime, oldStoredPath = f.VideoBlobPath;
            string path = $"videos/{f.Id}/{Guid.NewGuid():N}{newExt}";
            await ReplaceFileAsync(path, oldPath, s,
                () => { f.VideoExtension = newExt; f.VideoMime = newMime; f.VideoBlobPath = path; },
                () => { f.VideoExtension = oldExt; f.VideoMime = oldMime; f.VideoBlobPath = oldStoredPath; });
        }
    }
}
