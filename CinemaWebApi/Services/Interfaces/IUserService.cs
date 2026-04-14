using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(Guid id);

        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<IEnumerable<BookingResponse>> GetUserBookingsAsync(Guid id);
        Task<bool> UpdateProfileAsync(Guid id, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordRequest request);
        Task<bool> ToggleUserLockAsync(Guid id);
        Task<bool> UpdateUserRoleAsync(Guid id, string newRole);
    }
}