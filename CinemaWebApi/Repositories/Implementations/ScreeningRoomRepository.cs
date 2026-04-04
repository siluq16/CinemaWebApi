using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class ScreeningRoomRepository : IScreeningRoomRepository
    {
        private readonly CinemaWebApiContext _context;

        public ScreeningRoomRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScreeningRoom>> GetAllAsync()
        {
            return await _context.ScreeningRooms
                .Include(sr => sr.Cinema)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScreeningRoom>> GetByCinemaIdAsync(int cinemaId)
        {
            return await _context.ScreeningRooms
                .Include(sr => sr.Cinema)
                .Where(sr => sr.CinemaId == cinemaId)
                .ToListAsync();
        }

        public async Task<ScreeningRoom?> GetByIdAsync(int id)
        {
            return await _context.ScreeningRooms
                .Include(sr => sr.Cinema)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<ScreeningRoom> AddAsync(ScreeningRoom room)
        {
            await _context.ScreeningRooms.AddAsync(room);
            return room;
        }

        public void Update(ScreeningRoom room)
        {
            _context.ScreeningRooms.Update(room);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}