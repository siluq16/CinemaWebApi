using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimesController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _showtimeService.GetAllShowtimesAsync());
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingShowtimes()
        {
            return Ok(await _showtimeService.GetUpcomingShowtimesAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var showtime = await _showtimeService.GetShowtimeByIdAsync(id);
            if (showtime == null) return NotFound(new { message = "Không tìm thấy suất chiếu" });
            return Ok(showtime);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _showtimeService.CreateShowtimeAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}/seats")]
        public async Task<IActionResult> GetShowtimeSeats(Guid id)
        {
            var seats = await _showtimeService.GetShowtimeSeatsAsync(id);
            if (seats == null) return NotFound(new { message = "Không tìm thấy suất chiếu" });

            return Ok(seats);
        }

        // GET: api/Showtimes/search?date=2026-03-28&movieId=...&cinemaId=...
        [HttpGet("search")]
        public async Task<IActionResult> SearchShowtimes(
            [FromQuery] DateTime date,
            [FromQuery] Guid? movieId,
            [FromQuery] int? cinemaId)
        {
            try
            {
                var showtimes = await _showtimeService.GetShowtimesByDateAsync(date, movieId, cinemaId);

                return Ok(showtimes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy lịch chiếu", detail = ex.Message });
            }
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CancelShowtime(Guid id)
        {
            var isCancelled = await _showtimeService.CancelShowtimeAsync(id);
            if (!isCancelled) return NotFound(new { message = "Không tìm thấy suất chiếu" });

            return Ok(new { message = "Đã hủy suất chiếu thành công" });
        }
    }
}