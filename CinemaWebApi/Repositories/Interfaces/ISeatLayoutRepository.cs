using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface ISeatLayoutRepository
    {
        Task<IEnumerable<SeatLayout>> GetByRoomIdAsync(int roomId);
        Task<bool> HasSeatsAsync(int roomId); 
        Task AddRangeAsync(IEnumerable<SeatLayout> seats); 
        Task<bool> SaveChangesAsync();
    }
}