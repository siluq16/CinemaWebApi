using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieResponse>> GetAllMoviesAsync();
        Task<IEnumerable<MovieResponse>> GetNowShowingMoviesAsync();
        Task<IEnumerable<MovieResponse>> GetComingSoonMoviesAsync();
        Task<IEnumerable<MovieResponse>> GetFeaturedMoviesAsync();
        Task<MovieResponse?> GetMovieByIdAsync(Guid id);
        Task<MovieResponse> CreateMovieAsync(MovieRequest request);
        Task<MovieResponse?> UpdateMovieAsync(Guid id, MovieRequest request);
        Task<bool> DeleteMovieAsync(Guid id);
    }
}