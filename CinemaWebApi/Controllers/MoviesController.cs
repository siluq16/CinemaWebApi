using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            return Ok(await _movieService.GetAllMoviesAsync());
        }

        [HttpGet("now-showing")]
        public async Task<IActionResult> GetNowShowingMovies()
        {
            return Ok(await _movieService.GetNowShowingMoviesAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null) return NotFound(new { message = "Không tìm thấy phim" });
            return Ok(movie);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateMovie([FromBody] MovieRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _movieService.CreateMovieAsync(request);
            return CreatedAtAction(nameof(GetMovieById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] MovieRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _movieService.UpdateMovieAsync(id, request);
            if (updated == null) return NotFound(new { message = "Không tìm thấy phim để cập nhật" });

            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var isDeleted = await _movieService.DeleteMovieAsync(id);
            if (!isDeleted) return NotFound(new { message = "Không tìm thấy phim để xóa" });

            return Ok(new { message = "Xóa (hủy) phim thành công" });
        }
    }
}