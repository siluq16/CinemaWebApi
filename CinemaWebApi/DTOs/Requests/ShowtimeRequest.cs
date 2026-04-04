using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class CreateShowtimeRequest
    {
        [Required]
        public Guid MovieId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public string Language { get; set; } = "vi";
        public string SubtitleType { get; set; } = "vi"; 
        public string ScreenFormat { get; set; } = "2D"; 

        public string? Notes { get; set; }

    }
    
}