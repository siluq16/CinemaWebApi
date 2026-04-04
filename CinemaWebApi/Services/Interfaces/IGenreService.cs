using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreResponse>> GetAllGenresAsync();
        Task<GenreResponse?> GetGenreByIdAsync(int id);
        Task<GenreResponse> CreateGenreAsync(GenreRequest request);
        Task<GenreResponse?> UpdateGenreAsync(int id, GenreRequest request);
        Task<bool> DeleteGenreAsync(int id);
    }
}