using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatLayoutsController : ControllerBase
    {
        private readonly ISeatLayoutService _seatService;

        public SeatLayoutsController(ISeatLayoutService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetSeatsByRoom(int roomId)
        {
            var seats = await _seatService.GetSeatsByRoomIdAsync(roomId);
            return Ok(seats);
        }

        [HttpPost("generate")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GenerateSeats([FromBody] GenerateSeatsRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var seats = await _seatService.GenerateSeatsAsync(request);
                if (seats == null) return NotFound(new { message = "Không tìm thấy phòng chiếu (RoomId không hợp lệ)" });

                return Ok(new
                {
                    message = $"Tạo thành công {seats.Count()} ghế cho phòng chiếu",
                    data = seats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("update-seat-types")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSeatTypes([FromBody] IEnumerable<BatchUpdateSeatTypeRequest> request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _seatService.updateSeatTypesAsync(request);
                return Ok(new { message = "Cập nhật loại ghế thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}