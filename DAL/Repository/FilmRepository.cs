using Azure.Storage.Blobs;
using DAL.Context;
using DAL.Interfaces;
using Domain.Models;
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

        public async Task SavePosterAsync(Film f, Stream s)
        {
            //Постер с другим прошлым расширением не удалится, настроить эту работу через сервис
            BlobClient blobClient = _containerClient.GetBlobClient($"posters/{f.Id}.{f.PosterExtention}");
            await blobClient.UploadAsync(s, overwrite: true);
        }

        public async Task SaveVideoAsync(Film f, Stream s)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"videos/{f.Id}.{f.VideoExtention}");
            await blobClient.UploadAsync(s, overwrite: true);
        }

        public async Task SaveTrailerAsync(Film f, Stream s)
        {
            //Нужно перед этим увеличить количество трейлеров в сервисе
            BlobClient blobClient = _containerClient.GetBlobClient($"trailers/{f.Id}/{f.TrailerCount-1}.{f.TrailersExtentions.Last()}");
            await blobClient.UploadAsync(s, overwrite: true);
        }



        public async Task<Stream> GetPosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"posters/{f.Id}.{f.PosterExtention}");
            return await blobClient.OpenReadAsync();
        }

        public async Task<Stream> GetVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"videos/{f.Id}.{f.VideoExtention}");
            return await blobClient.OpenReadAsync();
        }

        public async Task<IEnumerable<Stream>> GetTrailersAsync(Film f)
        {
            List<Stream> res = new();
            for (int i = 0; i < f.TrailerCount; i++)
            {
                BlobClient blobClient = _containerClient.GetBlobClient($"trailers/{f.Id}/{i}.{f.TrailersExtentions[i]}");
                res.Add(await blobClient.OpenReadAsync());
            }
            return res;
        }



        public async Task DeletePosterAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"posters/{f.Id}.{f.PosterExtention}");
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteVideoAsync(Film f)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"videos/{f.Id}.{f.VideoExtention}");
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteTrailerAsync(Film f, int id)
        {
            BlobClient blobClient = _containerClient.GetBlobClient($"trailers/{f.Id}/{id}.{f.TrailersExtentions[id]}");
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteAllTrailersAsync(Film f)
        {
            for (int i = 0; i < f.TrailerCount; i++)
            {
                BlobClient blobClient = _containerClient.GetBlobClient($"trailers/{f.Id}/{i}.{f.TrailersExtentions[i]}");
                await blobClient.DeleteIfExistsAsync();
            }
        }
    }
}
