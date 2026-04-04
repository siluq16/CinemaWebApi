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
    }

    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!; 
        public string Token { get; set; } = null!; 
    }
}