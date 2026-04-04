using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    // 1. DTO cho từng món đồ ăn mà khách chọn
    public class BookingFoodItemRequest
    {
        [Required]
        public int FoodItemId { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Số lượng phải từ 1 đến 50")]
        public int Quantity { get; set; }
    }

    // 2. DTO Chính: Đơn đặt vé tổng hợp
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "Thiếu thông tin người dùng")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Thiếu thông tin suất chiếu")]
        public Guid ShowtimeId { get; set; }

        [Required(ErrorMessage = "Phải chọn ít nhất 1 ghế")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 ghế")]
        public List<int> SeatIds { get; set; } = new List<int>();

        // Danh sách đồ ăn (Có thể null hoặc rỗng nếu khách không mua bắp nước)
        public List<BookingFoodItemRequest>? FoodItems { get; set; }
    }
}