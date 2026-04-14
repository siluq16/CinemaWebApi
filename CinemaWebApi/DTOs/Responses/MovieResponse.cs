namespace CinemaWebApi.DTOs.Responses
{
    public class MovieResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? OriginalTitle { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public string Language { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal AvgRating { get; set; }
        public int AgeRating { get; set; }
        public string? BannerUrl { get; set; }
        public string? Country { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFeatured { get; set; }

        public List<string> Genres { get; set; } = new List<string>();
        public List<MovieCrewResponse> Crews { get; set; } = new List<MovieCrewResponse>();
    }

    public class MovieCrewResponse
    {
        public string Name { get; set; } = null!; 
        public string? CharacterName { get; set; }
        public string? RoleLabel { get; set; } 
        public string? PhotoUrl { get; set; }
    }
}