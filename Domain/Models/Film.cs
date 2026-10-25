namespace Domain.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Rating { get; set; }
<<<<<<< HEAD
        public string VideoExtension { get; set; } = string.Empty;
        public string VideoMime { get; set; } = string.Empty;
        public string PosterExtension { get; set; } = string.Empty;
        public string PosterMime {  get; set; } = string.Empty;
        public List<Trailer> Trailers { get; set; } = new(); 

        //public List<CrewMember> Crew
=======
        public int TrailerCount { get; set; }
        public string VideoExtention { get; set; } = string.Empty;
        public string PosterExtention { get; set; } = string.Empty;
        public List<string> TrailersExtentions { get; set; } = new List<string>();
>>>>>>> origin/main
    }
}
