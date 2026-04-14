using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class GenerateSeatsRequest
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        [Range(1, 26, ErrorMessage = "Số hàng chỉ được từ 1 đến 26 (Tương ứng A đến Z)")]
        public int RowCount { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Số ghế mỗi hàng từ 1 đến 50")]
        public int SeatsPerRow { get; set; }
    }

    public class BatchUpdateSeatTypeRequest
    {
        [Required]
        public List<int> SeatIds { get; set; } = new();

        [Required]
        [RegularExpression("^(standard|vip|couple|recliner|sweetbox)$")]
        public string SeatType { get; set; } = null!;
    }
}