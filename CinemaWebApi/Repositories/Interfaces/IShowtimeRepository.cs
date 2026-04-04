using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<UpcomingShowtime>> GetUpcomingShowtimesAsync();
        Task<Showtime?> GetByIdAsync(Guid id);
        Task<bool> HasOverlapAsync(int roomId, DateTime startTime, DateTime endTime);
        Task<Showtime> AddShowTimeAsync(Showtime showtime);
        Task<List<int>> GetBookedSeatIdsAsync(Guid showtimeId);
        Task<IEnumerable<UpcomingShowtime>> GetShowtimesByDateAsync(DateTime date, Guid? movieId = null, int? cinemaId = null);
        void UpShowTimedate(Showtime showtime);
        Task<bool> SaveChangesAsync();
    }
}