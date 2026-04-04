using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task<Genre> AddAsync(Genre genre);
        void Update(Genre genre);
        void Delete(Genre genre);
        Task<bool> SaveChangesAsync();
    }
}