using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class ScreeningRoomRequest
    {
        [Required(ErrorMessage = "Cinema ID không được để trống")]
        public int CinemaId { get; set; }

        [Required(ErrorMessage = "Tên phòng chiếu không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Loại phòng chiếu không được để trống")]
        [MaxLength(30)]
        public string RoomType { get; set; } = "2D"; // Mặc định là 2D

        [Required(ErrorMessage = "Tổng số ghế không được để trống")]
        [Range(1, 1000, ErrorMessage = "Số ghế phải lớn hơn 0")]
        public int TotalSeats { get; set; }

        public bool IsActive { get; set; } = true;
    }
}