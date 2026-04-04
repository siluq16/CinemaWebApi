using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class MovieRequest
    {
        [Required(ErrorMessage = "Tên phim không được để trống")]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [MaxLength(255)]
        public string? OriginalTitle { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Thời lượng phim phải lớn hơn 0")]
        public int DurationMinutes { get; set; }

        public DateOnly? ReleaseDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }

        [MaxLength(10)]
        public string Language { get; set; } = "vi";

        public string? BannerUrl { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        public bool IsFeatured { get; set; } = false; 

        public string Status { get; set; } = "coming_soon"; 

        public int AgeRating { get; set; } = 0; 

        public List<int> GenreIds { get; set; } = new List<int>();
        public List<MovieCrewRequest> Crews { get; set; } = new List<MovieCrewRequest>();
    }

    public class MovieCrewRequest
    {
        public Guid? DirectorId { get; set; }
        public Guid? CastMemberId { get; set; }
        public string? CharacterName { get; set; }
        public string? RoleLabel { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}