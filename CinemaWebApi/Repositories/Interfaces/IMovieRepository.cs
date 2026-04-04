using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<IEnumerable<Movie>> GetNowShowingMoviesAsync();
        Task<IEnumerable<Movie>> GetComingSoonMoviesAsync();
        Task<IEnumerable<Movie>> GetFeaturedMoviesAsync();
        Task<Movie?> GetByIdAsync(Guid id);
        Task<Movie> AddAsync(Movie movie);
        void Update(Movie movie);
        void Delete(Movie movie);
        Task<bool> SaveChangesAsync();
    }
}