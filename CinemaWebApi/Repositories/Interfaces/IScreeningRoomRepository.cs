using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IScreeningRoomRepository
    {
        Task<IEnumerable<ScreeningRoom>> GetAllAsync();
        Task<IEnumerable<ScreeningRoom>> GetByCinemaIdAsync(int cinemaId); // Lấy danh sách phòng theo rạp
        Task<ScreeningRoom?> GetByIdAsync(int id);
        Task<ScreeningRoom> AddAsync(ScreeningRoom room);
        void Update(ScreeningRoom room);
        Task<bool> SaveChangesAsync();
    }
}