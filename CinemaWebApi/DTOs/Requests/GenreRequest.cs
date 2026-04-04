using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class GenreRequest
    {
        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [MaxLength(50)]
        public string Slug { get; set; } = null!;
    }
}
