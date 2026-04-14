using CinemaWebApi.Data;
using CinemaWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CinemaWebApiContext _context;

        public ReviewRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieAsync(Guid movieId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.MovieId == movieId && r.IsVisible)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetUserReviewForMovieAsync(Guid userId, Guid movieId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            return review;
        }

        public async Task<Review?> GetReviewByIdAsync(Guid id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }

        public void DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
