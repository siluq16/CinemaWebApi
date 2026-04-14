using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // BƯỚC 1: TẠO BOOKING VÀ GIỮ GHẾ (Khách bấm "Tiếp tục" ở màn hình Chọn Ghế)
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var booking = await _bookingService.CreateBookingAsync(request);
                return Ok(booking);
            }
            catch (Exception ex)
            {
                var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new
                {
                    message = "Lỗi Database: " + innerError,
                    stackTrace = ex.StackTrace // Xóa dòng này đi sau khi debug xong nhé
                });
            }
        }

        // BƯỚC 2: THÊM ĐỒ ĂN (Khách đang ở màn hình chọn Bắp nước)
        [HttpPut("{id:guid}/food")]
        public async Task<IActionResult> AddFoodToBooking(Guid id, [FromBody] List<BookingFoodItemRequest> foodRequests)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedBooking = await _bookingService.AddFoodToBookingAsync(id, foodRequests);
                if (updatedBooking == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });

                return Ok(updatedBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{id:guid}/apply-promotion")]
        [Authorize] // Bắt buộc đăng nhập
        public async Task<IActionResult> ApplyPromotion(Guid id, [FromBody] ApplyPromotionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Lấy UserId từ Token JWT như đã làm ở hàm CreateBooking
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = Guid.Parse(userIdString);

                var updatedBooking = await _bookingService.ApplyPromotionAsync(id, userId, request.PromotionCode);
                return Ok(updatedBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}/remove-promotion")]
        [Authorize] // Yêu cầu phải đăng nhập
        public async Task<IActionResult> RemovePromotion(Guid id)
        {
            try
            {
                // Lấy ID của User đang đăng nhập từ Token
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out Guid userId))
                    return Unauthorized(new { message = "Không xác định được người dùng" });

                var updatedBooking = await _bookingService.RemovePromotionAsync(id, userId);

                return Ok(updatedBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // API LẤY CHI TIẾT BIÊN LAI (Để Frontend hiển thị Mã QR hoặc Màn hình Thanh toán)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingDetails(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });

            return Ok(booking);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("clear-pending/{showtimeId:guid}")]
        [Authorize]
        public async Task<IActionResult> ClearPending(Guid showtimeId)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = Guid.Parse(userIdString);

                await _bookingService.ClearMyPendingBookingsAsync(userId, showtimeId);

                return Ok(new { message = "Đã dọn dẹp giỏ hàng cũ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}