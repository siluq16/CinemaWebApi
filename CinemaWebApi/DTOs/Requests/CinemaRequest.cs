using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class CinemaRequest
    {
        [Required(ErrorMessage = "Tên rạp không được để trống")]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Thành phố không được để trống")]
        [MaxLength(50)]
        public string City { get; set; } = null!;

        [MaxLength(50)]
        public string? District { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
