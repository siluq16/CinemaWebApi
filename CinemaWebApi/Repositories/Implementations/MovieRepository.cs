using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class MovieRepository : IMovieRepository
    {
        private readonly CinemaWebApiContext _context;

        public MovieRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            // Include(m => m.Genres) để EF tự động JOIN bảng trung gian lấy ra thể loại
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.Director)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.CastMember)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetNowShowingMoviesAsync()
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.Director)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.CastMember)
                .Where(m => m.Status == "now_showing")
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetComingSoonMoviesAsync()
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.Director)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.CastMember)
                .Where(m => m.Status == "coming_soon")
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.Director)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.CastMember)
                .Where(m => m.IsFeatured)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }
        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.Director) 
                .Include(m => m.MovieCrews)
                    .ThenInclude(mc => mc.CastMember) 
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            return movie;
        }

        public void Update(Movie movie)
        {
            _context.Movies.Update(movie);
        }

        public void Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}