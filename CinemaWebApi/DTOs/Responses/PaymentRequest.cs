namespace CinemaWebApi.DTOs.Responses
{
    public class PaymentCallbackRequest
    {
        public Guid BookingId { get; set; }
        public string TransactionId { get; set; } = null!;
        public string Status { get; set; } = null!; // "00" là thành công, "99" là lỗi (ví dụ của VNPay)
        public string SecureHash { get; set; } = null!; // Chữ ký bảo mật để chống hacker giả mạo MoMo
    }
}
