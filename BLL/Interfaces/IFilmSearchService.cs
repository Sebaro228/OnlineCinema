using Domain.DTOs;

namespace BLL.Interfaces
{
    public interface IFilmSearchService
    {
        Task<FilmSearchResultDto> SearchAsync(FilmSearchDto filters, CancellationToken cancellationToken = default);
    }
}
