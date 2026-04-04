using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreeningRoomsController : ControllerBase
    {
        private readonly IScreeningRoomService _roomService;

        public ScreeningRoomsController(IScreeningRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _roomService.GetAllRoomsAsync());
        }

        // Tạo thêm Endpoint để lấy phòng theo Rạp
        [HttpGet("cinema/{cinemaId}")]
        public async Task<IActionResult> GetByCinemaId(int cinemaId)
        {
            return Ok(await _roomService.GetRoomsByCinemaIdAsync(cinemaId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound(new { message = "Không tìm thấy phòng chiếu" });
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] ScreeningRoomRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _roomService.CreateRoomAsync(request);
            if (created == null) return BadRequest(new { message = "Cinema ID không tồn tại" });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ScreeningRoomRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _roomService.UpdateRoomAsync(id, request);
            if (updated == null) return NotFound(new { message = "Không tìm thấy phòng chiếu hoặc Cinema ID không hợp lệ" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _roomService.DeleteRoomAsync(id);
            if (!isDeleted) return NotFound(new { message = "Không tìm thấy phòng chiếu để xóa" });
            return Ok(new { message = "Xóa (ẩn) phòng chiếu thành công" });
        }
    }
}