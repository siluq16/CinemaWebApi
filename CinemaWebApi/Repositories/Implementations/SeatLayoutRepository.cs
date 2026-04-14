using Azure.Core;
using CinemaWebApi.Data;
using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class SeatLayoutRepository : ISeatLayoutRepository
    {
        private readonly CinemaWebApiContext _context;

        public SeatLayoutRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeatLayout>> GetByRoomIdAsync(int roomId)
        {
            return await _context.SeatLayouts
                .Where(s => s.RoomId == roomId)
                .OrderBy(s => s.RowLabel)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<bool> HasSeatsAsync(int roomId)
        {
            return await _context.SeatLayouts.AnyAsync(s => s.RoomId == roomId);
        }

        public async Task AddRangeAsync(IEnumerable<SeatLayout> seats)
        {
            await _context.SeatLayouts.AddRangeAsync(seats);
        }

        public async Task UpdateSeatTypeAsync(IEnumerable<BatchUpdateSeatTypeRequest> seats)
        {
            foreach(var item in seats)
    {
                var seatsToUpdate = await _context.SeatLayouts
                    .Where(s => item.SeatIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var seat in seatsToUpdate)
                {
                    seat.SeatType = item.SeatType;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}