using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface ICinemaRepository
    {
        Task<IEnumerable<Cinema>> GetAllAsync();
        Task<Cinema> AddAsync(Cinema cinema);
        Task<bool> SaveChangesAsync();
        Task<Cinema?> GetByIdAsync(int id);
        void Update(Cinema cinema);
        void Delete(Cinema cinema);
    }
}
