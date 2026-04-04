using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class CastMemberService : ICastMemberService
    {
        private readonly ICastMemberRepository _repo;

        public CastMemberService(ICastMemberRepository repo)
        {
            _repo = repo;
        }

        private CastMemberResponse MapToResponse(CastMember c) => new CastMemberResponse
        {
            Id = c.Id,
            Name = c.Name,
            Bio = c.Bio,
            PhotoUrl = c.PhotoUrl,
            Nationality = c.Nationality,
            BirthDate = c.BirthDate
        };

        public async Task<IEnumerable<CastMemberResponse>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(MapToResponse);
        }

        public async Task<CastMemberResponse?> GetByIdAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            return c == null ? null : MapToResponse(c);
        }

        public async Task<CastMemberResponse> CreateAsync(CastMemberRequest request)
        {
            var c = new CastMember
            {
                Name = request.Name,
                Bio = request.Bio,
                PhotoUrl = request.PhotoUrl,
                Nationality = request.Nationality,
                BirthDate = request.BirthDate
            };
            await _repo.AddAsync(c);
            await _repo.SaveChangesAsync();
            return MapToResponse(c);
        }

        public async Task<CastMemberResponse?> UpdateAsync(Guid id, CastMemberRequest request)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;

            c.Name = request.Name; c.Bio = request.Bio; c.PhotoUrl = request.PhotoUrl;
            c.Nationality = request.Nationality; c.BirthDate = request.BirthDate;

            _repo.Update(c);
            await _repo.SaveChangesAsync();
            return MapToResponse(c);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return false;

            _repo.Delete(c);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}