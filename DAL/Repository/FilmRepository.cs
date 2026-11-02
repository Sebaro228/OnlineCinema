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



        public override async Task<Film?> GetAsync(string id)
            => await _context.Films
                .Include(f => f.Trailers)
                .FirstOrDefaultAsync(f => f.Id == id);

        public override async Task<IEnumerable<Film>> GetAllAsync()
            => await _dbSet.Include(f => f.Trailers).ToListAsync();



        public async Task UploadBlobAsync(string path, Stream stream)
        {
            var blob = _containerClient.GetBlobClient(path);
            await blob.UploadAsync(stream, overwrite: false);
        }

        public async Task DeleteBlobAsync(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                await _containerClient.GetBlobClient(path).DeleteIfExistsAsync();
        }

        public async Task SavePosterAsync(Film f, Stream s)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.PosterBlobPath);
            await blobClient.UploadAsync(s, overwrite: false);
        }

        public async Task SaveVideoAsync(Film f, Stream s)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.VideoBlobPath);
            await blobClient.UploadAsync(s, overwrite: false);
        }

        public async Task SaveTrailerDataAsync(Film f, Trailer t)
        {
            f.Trailers.Add(t);
            t.FilmId = f.Id;
            await _context.SaveChangesAsync();
        }

        public async Task SaveTrailerFileAsync(Trailer t, Stream s)
        {
            Film f = t.Film;
            BlobClient blobClient = _containerClient.GetBlobClient(t.BlobPath);
            await blobClient.UploadAsync(s, overwrite: false);
        }



        public async Task<Stream> GetPosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.PosterBlobPath);
            return await blobClient.OpenReadAsync();
        }

        public async Task<Stream> GetVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.VideoBlobPath);
            return await blobClient.OpenReadAsync();
        }

        public async Task<Trailer?> GetTrailerDataAsync(int trailerId)
            => _context.Trailers.Include(t => t.Film).FirstOrDefault(tr => tr.Id == trailerId);

        public async Task<Stream> GetTrailerFileAsync(Trailer t)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(t.BlobPath);
            return await blobClient.OpenReadAsync();
        }



        public async Task DeletePosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.PosterBlobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(f.VideoBlobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public void DeleteTrailerDataAsync(Trailer t)
        {
            _context.Trailers.Remove(t);
        }

        public async Task DeleteTrailerFileAsync(Trailer t)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(t.BlobPath);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteAllTrailersAsync(Film f)
        {
            foreach(Trailer t in f.Trailers)
            {
                BlobClient blobClient = _containerClient.GetBlobClient(t.BlobPath);
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
