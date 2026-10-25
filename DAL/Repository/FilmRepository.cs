using Azure.Storage.Blobs;
using DAL.Context;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DAL.Repository
{
    public class FilmRepository : Repository<Film>, IFilmRepository
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string ContainerName = "films";

        public FilmRepository(CinemaContext context, BlobServiceClient blobServiceClient) : base(context)
        {
             _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        }



        public override async Task<Film?> GetAsync(int id)
            => await _context.Films
                .Include(f => f.Trailers)
                .FirstOrDefaultAsync(f => f.Id == id);

        public override async Task<IEnumerable<Film>> GetAllAsync()
            => await _dbSet.Include(f => f.Trailers).ToListAsync();

        public override async Task DeleteAsync(Film item)
        {
            await base.DeleteAsync(item);

            if(!string.IsNullOrEmpty(item.PosterExtension))
                await DeletePosterAsync(item);
            if(item.Trailers.Any())
                await DeleteAllTrailersAsync(item);
            if(!string.IsNullOrEmpty(item.VideoExtension))
                await DeleteVideoAsync(item);
        }



        private string GetPosterBlobPath(Film f)
            => $"posters/{f.Id}{f.PosterExtension}";

        private string GetVideoBlobPath(Film f)
            => $"videos/{f.Id}{f.VideoExtension}";

        private string GetTrailerBlobPath(Film f, Trailer t)
            => $"trailers/{f.Id}/{t.Id}{t.Extension}";




        public async Task SavePosterAsync(Film f, Stream s)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetPosterBlobPath(f));
            await blobClient.UploadAsync(s, overwrite: true);
        }

        public async Task SaveVideoAsync(Film f, Stream s)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetVideoBlobPath(f));
            await blobClient.UploadAsync(s, overwrite: true);
        }

        public async Task SaveTrailerDataAsync(Film f, Trailer t)
        {
            f.Trailers.Add(t);
            t.FilmId = f.Id;
            await _context.SaveChangesAsync();
        }

        public async Task SaveTrailerFileAsync(Trailer t, Stream s)
        {
            Film? f = t.Film;
            if (f is null)
                throw new NullReferenceException("The film is null in film repository");
            BlobClient blobClient = _containerClient.GetBlobClient(GetTrailerBlobPath(f, t));
            await blobClient.UploadAsync(s, overwrite: true);
        }



        public async Task<Stream> GetPosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetPosterBlobPath(f));
            return await blobClient.OpenReadAsync();
        }

        public async Task<Stream> GetVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetVideoBlobPath(f));
            return await blobClient.OpenReadAsync();
        }

        public async Task<Trailer?> GetTrailerDataAsync(int trailerId)
            => _context.Trailers.FirstOrDefault(tr => tr.Id == trailerId);

        public async Task<Stream> GetTrailerFileAsync(int trailerId)
        {
            Trailer? t = _context.Trailers.FirstOrDefault(tr => tr.Id == trailerId);
            if (t is null)
                return Stream.Null;
            if (t.Film is null)
                throw new NullReferenceException("A film is null in the repository");
            BlobClient blobClient = _containerClient.GetBlobClient(GetTrailerBlobPath(t.Film, t));
            return await blobClient.OpenReadAsync();
        }



        public async Task DeletePosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetPosterBlobPath(f));
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(GetVideoBlobPath(f));
            await blobClient.DeleteIfExistsAsync();
        }

        public void DeleteTrailerDataAsync(Trailer t)
        {
            _context.Trailers.Remove(t);
        }

        public async Task DeleteTrailerFileAsync(Trailer t)
        {
            if (t.Film is null)
                throw new NullReferenceException("The film is null in film repository");
            BlobClient blobClient = _containerClient.GetBlobClient(GetTrailerBlobPath(t.Film, t));
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteAllTrailersAsync(Film f)
        {
            foreach(Trailer t in f.Trailers)
            {
                BlobClient blobClient = _containerClient.GetBlobClient(GetTrailerBlobPath(f, t));
                await blobClient.DeleteIfExistsAsync();
                _context.Trailers.Remove(t);
            }
            await _context.SaveChangesAsync();
        }



        public void UpdateTrailerData(Trailer t)
        {
            _context.Update(t);
        }
    }
}
