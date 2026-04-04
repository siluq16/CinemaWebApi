using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemaRepository _cinemaRepository;

        public CinemaService(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IEnumerable<CinemaResponse>> GetAllCinemasAsync()
        {
            var cinemas = await _cinemaRepository.GetAllAsync();

            return cinemas.Select(c => new CinemaResponse
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                City = c.City,
                District = c.District,
                Phone = c.Phone,
                Email = c.Email,
                OpenTime = c.OpenTime,
                CloseTime = c.CloseTime,
                IsActive = c.IsActive
            });
        }

        public async Task<CinemaResponse> CreateCinemaAsync(CinemaRequest request)
        {
            var newCinema = new Cinema
            {
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                District = request.District,
                Phone = request.Phone,
                Email = request.Email,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _cinemaRepository.AddAsync(newCinema);
            await _cinemaRepository.SaveChangesAsync();

            return new CinemaResponse
            {
                Id = newCinema.Id,
                Name = newCinema.Name,
                Address = newCinema.Address,
                City = newCinema.City,
                District = newCinema.District,
                Phone = newCinema.Phone,
                Email = newCinema.Email,
                OpenTime = newCinema.OpenTime,
                CloseTime = newCinema.CloseTime,
                IsActive = newCinema.IsActive
            };
        }

        public async Task<CinemaResponse?> GetCinemaByIdAsync(int id)
        {
            var c = await _cinemaRepository.GetByIdAsync(id);
            if (c == null) return null;

            return new CinemaResponse
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                City = c.City,
                District = c.District,
                Phone = c.Phone,
                Email = c.Email,
                OpenTime = c.OpenTime,
                CloseTime = c.CloseTime,
                IsActive = c.IsActive
            };
        }

        public async Task<CinemaResponse?> UpdateCinemaAsync(int id, CinemaRequest request)
        {
            var cinema = await _cinemaRepository.GetByIdAsync(id);
            if (cinema == null) return null;

            cinema.Name = request.Name;
            cinema.Address = request.Address;
            cinema.City = request.City;
            cinema.District = request.District;
            cinema.Phone = request.Phone;
            cinema.Email = request.Email;
            cinema.OpenTime = request.OpenTime;
            cinema.CloseTime = request.CloseTime;
            cinema.IsActive = request.IsActive;

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.SaveChangesAsync();

            return await GetCinemaByIdAsync(id);
        }

        public async Task<bool> DeleteCinemaAsync(int id)
        {
            var cinema = await _cinemaRepository.GetByIdAsync(id);
            if (cinema == null) return false; 

            cinema.IsActive = false;
            _cinemaRepository.Update(cinema);

            // _cinemaRepository.Delete(cinema);

            await _cinemaRepository.SaveChangesAsync();
            return true;
        }
    }
}