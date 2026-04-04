using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class DirectorRequest
    {
        [Required(ErrorMessage = "Tên đạo diễn không được để trống")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        public DateOnly? BirthDate { get; set; }
    }
}