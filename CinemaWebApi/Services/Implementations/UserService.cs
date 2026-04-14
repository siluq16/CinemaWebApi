using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,     
            LastLoginAt = u.LastLoginAt
        };
        public async Task<UserResponse?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToResponse(user);
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                AvatarUrl = u.AvatarUrl,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u?.LastLoginAt
            }).OrderByDescending(u => u.CreatedAt);
        }

        public async Task<IEnumerable<BookingResponse>> GetUserBookingsAsync(Guid id)
        {
            var bookings = await _userRepository.GetBookingsByUserIdAsync(id);

            return bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode ?? "N/A",
                CreatedAt = b.CreatedAt,
                StartTime = b.Showtime?.StartTime ?? DateTime.MinValue,

                MovieTitle = b.Showtime?.Movie?.Title ?? "N/A",
                CinemaName = b.Showtime?.Room?.Cinema?.Name ?? "N/A",
                RoomName = b.Showtime?.Room?.Name ?? "N/A",
                Status = b.Status ?? "pending",

                TicketTotal = b.TotalAmount,
                FoodTotal = b.FoodAmount,
                SubTotal = b.TotalAmount + b.FoodAmount,
                DiscountAmount = b.DiscountAmount,
                FinalAmount = b.FinalAmount,

                Seats = b.BookingSeats != null && b.BookingSeats.Any()
                    ? b.BookingSeats.Select(bs => new BookingSeatResponse
                    {
                        SeatId = bs.SeatId,
                        RowLabel = bs.Seat?.RowLabel ?? "",
                        SeatNumber = bs.Seat?.SeatNumber ?? 0,
                        Price = bs.Price
                    }).ToList()
                    : new List<BookingSeatResponse>(),

                FoodItems = b.FoodOrders != null && b.FoodOrders.Any()
                    ? b.FoodOrders.SelectMany(fo => fo.FoodOrderItems)
                                  .Select(foi => new BookingFoodResponse
                                  {
                                      FoodItemId = foi.FoodItemId,
                                      FoodName = foi.FoodItem?.Name ?? "N/A",
                                      Quantity = foi.Quantity,
                                      UnitPrice = foi.UnitPrice,
                                      SubTotal = foi.Quantity * foi.UnitPrice
                                  }).ToList()
                    : new List<BookingFoodResponse>()
            });
        }

        public async Task<bool> UpdateProfileAsync(Guid id, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id); // Gọi repository lấy user
            if (user == null) return false;

            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.DateOfBirth = request.DateOfBirth;
            user.Gender = request.Gender;
            user.AvatarUrl = request.AvatarUrl;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);

            if (!isPasswordValid)
            {
                return false; 
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ToggleUserLockAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = !user.IsActive; // Đảo ngược trạng thái
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid id, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Role = newRole;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}