using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly CinemaWebApiContext _context;

        public DirectorRepository(CinemaWebApiContext context) { _context = context; }

        public async Task<IEnumerable<Director>> GetAllAsync() => await _context.Directors.ToListAsync();
        public async Task<Director?> GetByIdAsync(Guid id) => await _context.Directors.FindAsync(id);
        public async Task<Director> AddAsync(Director director)
        {
            await _context.Directors.AddAsync(director);
            return director;
        }
        public void Update(Director director) => _context.Directors.Update(director);
        public void Delete(Director director) => _context.Directors.Remove(director);
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}