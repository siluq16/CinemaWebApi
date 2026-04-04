using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class SeatLayoutService : ISeatLayoutService
    {
        private readonly ISeatLayoutRepository _seatRepository;
        private readonly IScreeningRoomRepository _roomRepository; 

        public SeatLayoutService(ISeatLayoutRepository seatRepository, IScreeningRoomRepository roomRepository)
        {
            _seatRepository = seatRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<SeatLayoutResponse>> GetSeatsByRoomIdAsync(int roomId)
        {
            var seats = await _seatRepository.GetByRoomIdAsync(roomId);
            return seats.Select(s => new SeatLayoutResponse
            {
                Id = s.Id,
                RoomId = s.RoomId,
                RowLabel = s.RowLabel,
                SeatNumber = s.SeatNumber,
                SeatType = s.SeatType,
                XPosition = s.XPosition,
                YPosition = s.YPosition,
                IsActive = s.IsActive
            });
        }

        public async Task<IEnumerable<SeatLayoutResponse>?> GenerateSeatsAsync(GenerateSeatsRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            if (room == null) return null;

            if (await _seatRepository.HasSeatsAsync(request.RoomId))
            {
                throw new Exception("Phòng chiếu này đã có sơ đồ ghế. Không thể tạo tự động lần nữa.");
            }

            var newSeats = new List<SeatLayout>();

            for (int i = 0; i < request.RowCount; i++)
            {
                char rowLabel = (char)('A' + i);

                for (int j = 1; j <= request.SeatsPerRow; j++)
                {
                    string seatType = "standard";
                    if (i == request.RowCount - 1) seatType = "couple";
                    else if (i >= request.RowCount / 3 && i <= request.RowCount * 2 / 3) seatType = "vip";

                    newSeats.Add(new SeatLayout
                    {
                        RoomId = request.RoomId,
                        RowLabel = rowLabel.ToString(),
                        SeatNumber = j,
                        SeatType = seatType,

                        XPosition = j,       
                        YPosition = i + 1, 

                        IsActive = true
                    });
                }
            }

            await _seatRepository.AddRangeAsync(newSeats);
            await _seatRepository.SaveChangesAsync();

            return await GetSeatsByRoomIdAsync(request.RoomId);
        }
    }
}