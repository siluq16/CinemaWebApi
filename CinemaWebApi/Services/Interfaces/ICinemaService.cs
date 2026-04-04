using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface ICinemaService
    {
        Task<IEnumerable<CinemaResponse>> GetAllCinemasAsync();
        Task<CinemaResponse> CreateCinemaAsync(CinemaRequest request);
        Task<CinemaResponse?> GetCinemaByIdAsync(int id);
        Task<CinemaResponse?> UpdateCinemaAsync(int id, CinemaRequest request);
        Task<bool> DeleteCinemaAsync(int id);
    }
}