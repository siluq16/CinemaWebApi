using CinemaWebApi.Data;
using CinemaWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsByMovieAsync(Guid movieId);
        Task<Review?> GetUserReviewForMovieAsync(Guid userId, Guid movieId);
        Task<Review> AddReviewAsync(Review review);
        Task<Review?> GetReviewByIdAsync(Guid id);
        void DeleteReview(Review review);
        Task<bool> SaveChangesAsync();
    }

}