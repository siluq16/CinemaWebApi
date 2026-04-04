using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorRepository _repo;

        public DirectorService(IDirectorRepository repo) { _repo = repo; }

        private DirectorResponse MapToResponse(Director d) => new DirectorResponse
        {
            Id = d.Id,
            Name = d.Name,
            Bio = d.Bio,
            PhotoUrl = d.PhotoUrl,
            Nationality = d.Nationality,
            BirthDate = d.BirthDate
        };

        public async Task<IEnumerable<DirectorResponse>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(MapToResponse);
        }

        public async Task<DirectorResponse?> GetByIdAsync(Guid id)
        {
            var d = await _repo.GetByIdAsync(id);
            return d == null ? null : MapToResponse(d);
        }

        public async Task<DirectorResponse> CreateAsync(DirectorRequest request)
        {
            var d = new Director
            {
                Name = request.Name,
                Bio = request.Bio,
                PhotoUrl = request.PhotoUrl,
                Nationality = request.Nationality,
                BirthDate = request.BirthDate
            };
            await _repo.AddAsync(d);
            await _repo.SaveChangesAsync();
            return MapToResponse(d);
        }

        public async Task<DirectorResponse?> UpdateAsync(Guid id, DirectorRequest request)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return null;

            d.Name = request.Name; d.Bio = request.Bio; d.PhotoUrl = request.PhotoUrl;
            d.Nationality = request.Nationality; d.BirthDate = request.BirthDate;

            _repo.Update(d);
            await _repo.SaveChangesAsync();
            return MapToResponse(d);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d == null) return false;

            _repo.Delete(d);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}