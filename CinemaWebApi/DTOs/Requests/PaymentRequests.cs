using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class ProcessPaymentRequest
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [RegularExpression("^(vnpay|momo|zalopay|credit_card|cash)$", ErrorMessage = "Phương thức thanh toán không hợp lệ")]
        public string Method { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        public decimal Amount { get; set; }

        // TransactionId giả lập từ phía Frontend hoặc Gateway trả về
        public string? TransactionId { get; set; }
    }
}