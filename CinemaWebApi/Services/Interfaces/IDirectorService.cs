using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IDirectorService
    {
        Task<IEnumerable<DirectorResponse>> GetAllAsync();
        Task<DirectorResponse?> GetByIdAsync(Guid id);
        Task<DirectorResponse> CreateAsync(DirectorRequest request);
        Task<DirectorResponse?> UpdateAsync(Guid id, DirectorRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}