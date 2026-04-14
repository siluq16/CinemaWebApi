using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface ISeatLayoutService
    {
        Task<IEnumerable<SeatLayoutResponse>> GetSeatsByRoomIdAsync(int roomId);
        Task<IEnumerable<SeatLayoutResponse>?> GenerateSeatsAsync(GenerateSeatsRequest request);
        Task updateSeatTypesAsync(IEnumerable<BatchUpdateSeatTypeRequest> request);
    }
}