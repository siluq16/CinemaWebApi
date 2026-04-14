using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetMovieReviewsAsync(Guid movieId);
        Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request);
        Task<ReviewResponse> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewRequest request);
        Task DeleteReviewAsync(Guid userId, Guid reviewId);
    }
}
