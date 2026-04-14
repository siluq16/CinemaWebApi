using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Implementations;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userRepository;

        public UsersController(IUserService userService)
        {
            _userRepository = userService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng" });
            return Ok(user);
        }
        [HttpGet]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:guid}/bookings")]
        [Authorize] 
        public async Task<IActionResult> GetUserBookings(Guid id)
        {
            var bookings = await _userRepository.GetUserBookingsAsync(id);
            return Ok(bookings);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var isSuccess = await _userRepository.UpdateProfileAsync(id, request);
                if (!isSuccess) return NotFound(new { message = "Không tìm thấy tài khoản" });

                return Ok(new { message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var isSuccess = await _userRepository.ChangePasswordAsync(id, request);

                if (!isSuccess)
                    return BadRequest(new { message = "Đổi mật khẩu thất bại. Mật khẩu cũ không chính xác." });

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/toggle-lock")]
        [Authorize(Roles = "admin")] // Chỉ admin mới được khóa
        public async Task<IActionResult> ToggleUserLock(Guid id)
        {
            var isSuccess = await _userRepository.ToggleUserLockAsync(id);
            if (!isSuccess) return NotFound(new { message = "Không tìm thấy người dùng." });
            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        [HttpPatch("{id:guid}/role")]
        [Authorize(Roles = "admin")] // Chỉ admin mới được đổi quyền
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var isSuccess = await _userRepository.UpdateUserRoleAsync(id, request.Role);
            if (!isSuccess) return NotFound(new { message = "Không tìm thấy người dùng." });
            return Ok(new { message = "Cập nhật quyền thành công." });
        }
    }
}