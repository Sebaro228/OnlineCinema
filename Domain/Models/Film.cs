namespace Domain.Models
{
    public class Film
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
        public string VideoExtension { get; set; } = string.Empty;
        public string VideoMime { get; set; } = string.Empty;
        public string PosterExtension { get; set; } = string.Empty;
        public string PosterMime { get; set; } = string.Empty;
        public string PosterBlobPath { get; set; } = string.Empty;
        public string VideoBlobPath { get; set; } = string.Empty;
        public List<Trailer> Trailers { get; set; } = new();
        //public List<CrewMember> Crew
    }
}
