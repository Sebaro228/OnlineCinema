using Domain.Models;

namespace Domain.DTOs
{
    public class FilmSearchResultDto
    {
        public List<Film> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => TotalCount == 0 ? 0 : (TotalCount - 1) / PageSize + 1;
    }
}
