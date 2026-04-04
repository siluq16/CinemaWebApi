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
        private readonly IBookingRepository _bookingRepo; // Dùng để check xem khách mua vé thật chưa

        public ReviewService(IReviewRepository reviewRepo, IBookingRepository bookingRepo)
        {
            _reviewRepo = reviewRepo;
            _bookingRepo = bookingRepo;
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
                CreatedAt = r.CreatedAt
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

            // 3. Tạo Review
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
    }
}
