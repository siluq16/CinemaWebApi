using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class CreateReviewRequest
    {
        [Required]
        public Guid MovieId { get; set; }

        public Guid? BookingId { get; set; } // Nếu truyền vào thì sẽ hiện mác "Đã mua vé"

        [Required]
        [Range(1, 10, ErrorMessage = "Điểm đánh giá phải từ 1 đến 10")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
    public class UpdateReviewRequest
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}