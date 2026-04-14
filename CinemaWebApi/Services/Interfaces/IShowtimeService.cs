using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IShowtimeService
    {
        Task<IEnumerable<ShowtimeResponse>> GetAllShowtimesAsync();
        Task<IEnumerable<ShowtimeResponse>> GetUpcomingShowtimesAsync();
        Task<ShowtimeResponse?> GetShowtimeByIdAsync(Guid id);
        Task<ShowtimeResponse> CreateShowtimeAsync(CreateShowtimeRequest request);
        Task<IEnumerable<ShowtimeSeatResponse>?> GetShowtimeSeatsAsync(Guid showtimeId);
        Task<IEnumerable<ShowtimeResponse>> GetShowtimesByDateAsync(DateTime date, Guid? movieId = null, int? cinemaId = null);
        Task<bool> CancelShowtimeAsync(Guid id); // Hủy lịch chiếu
    }
}