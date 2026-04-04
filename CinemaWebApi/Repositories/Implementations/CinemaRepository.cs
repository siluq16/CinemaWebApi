using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class CinemaRepository : ICinemaRepository
    {
        private readonly CinemaWebApiContext _context;

        public CinemaRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cinema>> GetAllAsync()
        {
            return await _context.Cinemas
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Cinema> AddAsync(Cinema cinema)
        {
            await _context.Cinemas.AddAsync(cinema);
            return cinema;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Cinema?> GetByIdAsync(int id)
        {
            return await _context.Cinemas.FindAsync(id);
        }

        public void Update(Cinema cinema)
        {
            _context.Cinemas.Update(cinema);
        }

        public void Delete(Cinema cinema)
        {
            _context.Cinemas.Remove(cinema);
        }
    }
}