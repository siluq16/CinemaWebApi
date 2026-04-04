using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _genreService.GetAllGenresAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null) return NotFound(new { message = "Không tìm thấy thể loại" });
            return Ok(genre);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] GenreRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _genreService.CreateGenreAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] GenreRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _genreService.UpdateGenreAsync(id, request);
            if (updated == null) return NotFound(new { message = "Không tìm thấy thể loại để cập nhật" });
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _genreService.DeleteGenreAsync(id);
            if (!isDeleted) return NotFound(new { message = "Không tìm thấy thể loại để xóa" });
            return Ok(new { message = "Xóa thể loại thành công" });
        }
    }
}