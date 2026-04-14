using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Implementations;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IMovieService _movieService;

        public ReviewService(IReviewRepository reviewRepo, IBookingRepository bookingRepo, IMovieService movieService)
        {
            _reviewRepo = reviewRepo;
            _bookingRepo = bookingRepo;
            _movieService = movieService;
        }

        public async Task<IEnumerable<ReviewResponse>> GetMovieReviewsAsync(Guid movieId)
        {
            var reviews = await _reviewRepo.GetReviewsByMovieAsync(movieId);
            return reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? "Ẩn danh",
                UserAvatar = r.User?.AvatarUrl,
                Rating = r.Rating,
                Comment = r.Comment,
                IsVerified = r.IsVerified,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
        }

        public async Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request)
        {
            var existingReview = await _reviewRepo.GetUserReviewForMovieAsync(userId, request.MovieId);
            if (existingReview != null)
                throw new Exception("Bạn đã đánh giá bộ phim này rồi.");

            bool isVerified = false;
            if (request.BookingId.HasValue)
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(request.BookingId.Value);
                if (booking != null &&
                    booking.UserId == userId &&
                    booking.Showtime?.MovieId == request.MovieId &&
                    booking.Status == "confirmed")
                {
                    isVerified = true;
                }
            }

            var review = new Review
            {
                UserId = userId,
                MovieId = request.MovieId,
                BookingId = isVerified ? request.BookingId : null,
                Rating = request.Rating,
                Comment = request.Comment,
                IsVerified = isVerified,
                IsVisible = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _reviewRepo.AddReviewAsync(review);

            await _reviewRepo.SaveChangesAsync();
            await _movieService.UpdateMovieRatingAsync(request.MovieId);

            return new ReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = "Bạn",
                Rating = review.Rating,
                Comment = review.Comment,
                IsVerified = review.IsVerified,
                CreatedAt = review.CreatedAt
            };
        }
        public async Task<ReviewResponse> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewRequest request)
        {
            var review = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (review == null) throw new Exception("Không tìm thấy đánh giá.");
            if (review.UserId != userId) throw new Exception("Bạn không có quyền sửa đánh giá này.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedAt = DateTime.Now;

            await _reviewRepo.SaveChangesAsync();

            await _movieService.UpdateMovieRatingAsync(review.MovieId);

            return new ReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = "Bạn",
                Rating = review.Rating,
                Comment = review.Comment,
                IsVerified = review.IsVerified,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        public async Task DeleteReviewAsync(Guid userId, Guid reviewId)
        {
            var review = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (review == null) throw new Exception("Không tìm thấy đánh giá.");
            if (review.UserId != userId) throw new Exception("Bạn không có quyền xóa đánh giá này.");

            var movieId = review.MovieId;
            _reviewRepo.DeleteReview(review);
            await _reviewRepo.SaveChangesAsync();

            await _movieService.UpdateMovieRatingAsync(movieId);
        }
    }
}
