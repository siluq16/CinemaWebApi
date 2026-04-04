using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IScreeningRoomService
    {
        Task<IEnumerable<ScreeningRoomResponse>> GetAllRoomsAsync();
        Task<IEnumerable<ScreeningRoomResponse>> GetRoomsByCinemaIdAsync(int cinemaId);
        Task<ScreeningRoomResponse?> GetRoomByIdAsync(int id);
        Task<ScreeningRoomResponse?> CreateRoomAsync(ScreeningRoomRequest request);
        Task<ScreeningRoomResponse?> UpdateRoomAsync(int id, ScreeningRoomRequest request);
        Task<bool> DeleteRoomAsync(int id); 
    }
}