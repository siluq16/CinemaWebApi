using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class PricingRuleRequest
    {
        [Required(ErrorMessage = "Tên luật không được để trống")]
        public string RuleName { get; set; } = null!;

        public string? SeatType { get; set; } 
        public string? ScreenFormat { get; set; } 
        public TimeOnly? StartTimeFilter { get; set; }
        public TimeOnly? EndTimeFilter { get; set; }
        public string? DayOfWeekFilter { get; set; } 
        public bool IsHoliday { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tiền không hợp lệ")]
        public decimal BasePrice { get; set; }

        [Required]
        public int Priority { get; set; } // Số càng to độ ưu tiên càng cao
    }

    public class HolidayRequest
    {
        [Required(ErrorMessage = "Ngày lễ không được để trống")]
        public DateOnly HolidayDate { get; set; }

        [Required(ErrorMessage = "Tên/Mô tả ngày lễ không được để trống")]
        public string Description { get; set; } = null!;
    }
}