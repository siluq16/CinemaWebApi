namespace CinemaWebApi.DTOs.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Role { get; set; } = null!;
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!; 
        public string Token { get; set; } = null!; 
        public string AvatarUrl { get; set; } = null!;
    }

    public class BookingHistoryResponse
    {
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public DateTime BookingDate { get; set; } // Map với created_at
        public DateTime? ShowtimeStart { get; set; } // Map với start_time của showtime
        public string MovieTitle { get; set; } = null!;
        public string CinemaName { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public string SeatNumbers { get; set; } = null!; // Ví dụ: "G1, G2"
        public decimal TotalAmount { get; set; } // Map với final_amount
        public string Status { get; set; } = null!;
    }
}