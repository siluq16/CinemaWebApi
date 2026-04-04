using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "Thiếu Token từ Google")]
        public string IdToken { get; set; } = null!;
    }
}