using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        // GET: api/cinemas
        [HttpGet]
        public async Task<IActionResult> GetAllCinemas()
        {
            try
            {
                var cinemas = await _cinemaService.GetAllCinemasAsync();
                return Ok(cinemas); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // POST: api/cinemas
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCinema([FromBody] CinemaRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCinema = await _cinemaService.CreateCinemaAsync(request);

                return CreatedAtAction(nameof(GetAllCinemas), new { id = createdCinema.Id }, createdCinema);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo rạp chiếu", detail = ex.Message });
            }
        }

        // GET: api/cinemas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCinemaById(int id)
        {
            var cinema = await _cinemaService.GetCinemaByIdAsync(id);
            if (cinema == null)
            {
                return NotFound(new { message = $"Không tìm thấy rạp chiếu với ID = {id}" });
            }
            return Ok(cinema);
        }

        // PUT: api/cinemas/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCinema(int id, [FromBody] CinemaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedCinema = await _cinemaService.UpdateCinemaAsync(id, request);
                if (updatedCinema == null)
                {
                    return NotFound(new { message = $"Không tìm thấy rạp chiếu với ID = {id}" });
                }

                return Ok(updatedCinema);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật rạp chiếu", detail = ex.Message });
            }
        }

        // DELETE: api/cinemas/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCinema(int id)
        {
            try
            {
                var isDeleted = await _cinemaService.DeleteCinemaAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = $"Không tìm thấy rạp chiếu với ID = {id}" });
                }

                return Ok(new { message = "Xóa rạp chiếu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa rạp chiếu", detail = ex.Message });
            }
        }
    }
}