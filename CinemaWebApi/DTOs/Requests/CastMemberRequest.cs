using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class CastMemberRequest
    {
        [Required(ErrorMessage = "Tên diễn viên không được để trống")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        public DateOnly? BirthDate { get; set; }
    }
}