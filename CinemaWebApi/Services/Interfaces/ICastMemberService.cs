using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface ICastMemberService
    {
        Task<IEnumerable<CastMemberResponse>> GetAllAsync();
        Task<CastMemberResponse?> GetByIdAsync(Guid id);
        Task<CastMemberResponse> CreateAsync(CastMemberRequest request);
        Task<CastMemberResponse?> UpdateAsync(Guid id, CastMemberRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}