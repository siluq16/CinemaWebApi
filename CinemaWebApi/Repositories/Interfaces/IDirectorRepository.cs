using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> GetAllAsync();
        Task<Director?> GetByIdAsync(Guid id);
        Task<Director> AddAsync(Director director);
        void Update(Director director);
        void Delete(Director director);
        Task<bool> SaveChangesAsync();
    }
}