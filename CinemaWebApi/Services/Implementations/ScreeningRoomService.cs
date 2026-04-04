using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class ScreeningRoomService : IScreeningRoomService
    {
        private readonly IScreeningRoomRepository _roomRepository;
        private readonly ICinemaRepository _cinemaRepository;

        public ScreeningRoomService(IScreeningRoomRepository roomRepository, ICinemaRepository cinemaRepository)
        {
            _roomRepository = roomRepository;
            _cinemaRepository = cinemaRepository;
        }

        private ScreeningRoomResponse MapToResponse(ScreeningRoom room)
        {
            return new ScreeningRoomResponse
            {
                Id = room.Id,
                CinemaId = room.CinemaId,
                CinemaName = room.Cinema?.Name ?? "N/A", 
                Name = room.Name,
                RoomType = room.RoomType,
                TotalSeats = room.TotalSeats,
                IsActive = room.IsActive
            };
        }

        public async Task<IEnumerable<ScreeningRoomResponse>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.Select(MapToResponse);
        }

        public async Task<IEnumerable<ScreeningRoomResponse>> GetRoomsByCinemaIdAsync(int cinemaId)
        {
            var rooms = await _roomRepository.GetByCinemaIdAsync(cinemaId);
            return rooms.Select(MapToResponse);
        }

        public async Task<ScreeningRoomResponse?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;
            return MapToResponse(room);
        }

        public async Task<ScreeningRoomResponse?> CreateRoomAsync(ScreeningRoomRequest request)
        {
            var cinema = await _cinemaRepository.GetByIdAsync(request.CinemaId);
            if (cinema == null) return null;

            var newRoom = new ScreeningRoom
            {
                CinemaId = request.CinemaId,
                Name = request.Name,
                RoomType = request.RoomType,
                TotalSeats = request.TotalSeats,
                IsActive = request.IsActive
            };

            await _roomRepository.AddAsync(newRoom);
            await _roomRepository.SaveChangesAsync();

            return await GetRoomByIdAsync(newRoom.Id);
        }

        public async Task<ScreeningRoomResponse?> UpdateRoomAsync(int id, ScreeningRoomRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            if (room.CinemaId != request.CinemaId)
            {
                var cinema = await _cinemaRepository.GetByIdAsync(request.CinemaId);
                if (cinema == null) return null;
            }

            room.CinemaId = request.CinemaId;
            room.Name = request.Name;
            room.RoomType = request.RoomType;
            room.TotalSeats = request.TotalSeats;
            room.IsActive = request.IsActive;

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return await GetRoomByIdAsync(id);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return false;

            room.IsActive = false;
            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return true;
        }
    }
}