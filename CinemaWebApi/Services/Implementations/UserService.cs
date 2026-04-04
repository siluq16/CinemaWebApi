using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private UserResponse MapToResponse(User u) => new UserResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            AvatarUrl = u.AvatarUrl,
            DateOfBirth = u.DateOfBirth,
            Gender = u.Gender,
            Role = u.Role,
            IsVerified = u.IsVerified,
            IsActive = u.IsActive
        };

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Kiểm tra Email và Phone đã tồn tại chưa
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                throw new Exception("Email này đã được đăng ký.");

            if (await _userRepository.GetByPhoneAsync(request.Phone) != null)
                throw new Exception("Số điện thoại này đã được đăng ký.");

            // 2. Hash mật khẩu (Tạm thời dùng Base64 cho nhanh, thực tế nên dùng BCrypt)
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(request.Password);
            string hashedPassword = Convert.ToBase64String(plainTextBytes);

            // 3. Tạo User
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = hashedPassword,
                Role = "customer", // Mặc định là khách hàng
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToResponse(user);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToResponse(user);
        }
    }
}