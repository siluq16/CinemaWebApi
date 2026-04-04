using System.Security.Claims;
using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("movie/{movieId:guid}")]
        [AllowAnonymous] // Ai cũng xem được đánh giá
        public async Task<IActionResult> GetMovieReviews(Guid movieId)
        {
            var reviews = await _reviewService.GetMovieReviewsAsync(movieId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize] // Phải đăng nhập mới được đánh giá
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await _reviewService.CreateReviewAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}