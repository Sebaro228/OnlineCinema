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
    public class FilmService : IFilmService
    {
        //Постер с другим прошлым расширением не удалится, настроить эту работу через сервис
        //Нужно перед этим увеличить количество трейлеров в сервисе
        //Добавить ли всем методам статусы (bool, false) или не добавлять никому

        private readonly FilmRepository _repository;

        public FilmService(FilmRepository repository)
        {
            _repository = repository;
        }

        public Task CratePosterAsync(Film f, Stream s)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Film item)
        {
            await _repository.CreateAsync(item);
        }

        public Task CreateTrailerAsync(Film f, Stream s)
        {
            throw new NotImplementedException();
        }

        public Task CreateVideoAsync(Film f, Stream s)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> DeleteAllTrailersAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool, string)> DeleteAsync(int id)
        {
            Film? f = await _repository.GetAsync(id);
            if (f is null)
                return (false, "Film was not found");
            await _repository.DeleteAsync(f);
            return (true, "Success");
        }

        public Task<(bool, string)> DeletePosterAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> DeleteTrailerAsync(Film f, int id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> DeleteVideoAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public async Task<Film?> FirstOrDefaultAsync(Expression<Func<Film, bool>> pred)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Film>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Film?> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetPosterAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Stream>> GetTrailersAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetVideoAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Film item)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePosterAsync(Film f)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTrailerAsync(Film f, int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVideoAsync(Film f)
        {
            throw new NotImplementedException();
        }
    }
}
