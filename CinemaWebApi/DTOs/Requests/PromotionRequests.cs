using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class ApplyPromotionRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập mã khuyến mãi")]
        public string PromotionCode { get; set; } = null!;
    }

    public class CreatePromotionRequest
    {
        [Required(ErrorMessage = "Mã khuyến mãi không được để trống")]
        [MaxLength(30, ErrorMessage = "Mã không được vượt quá 30 ký tự")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(150)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        [RegularExpression("^(percentage|fixed_amount)$", ErrorMessage = "Loại giảm giá phải là 'percentage' hoặc 'fixed_amount'")]
        public string DiscountType { get; set; } = null!;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ")]
        public decimal MinOrderValue { get; set; } = 0;

        public bool AppliesToFood { get; set; } = false;

        public int? MaxUses { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "Số lượt dùng tối đa mỗi user phải từ 1 trở lên")]
        public int MaxUsesPerUser { get; set; } = 1;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }
    }
}