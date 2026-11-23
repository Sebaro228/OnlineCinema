namespace Domain.DTOs
{
    // Search fields match FilmDto; nullable values mean that the filter is not applied.
    public class FilmSearchDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Year { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
