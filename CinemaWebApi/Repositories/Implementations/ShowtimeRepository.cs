using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly CinemaWebApiContext _context;

        public ShowtimeRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UpcomingShowtime>> GetUpcomingShowtimesAsync()
        {
            return await _context.UpcomingShowtimes
                .ToListAsync();
        }

        public async Task<Showtime?> GetByIdAsync(Guid id)
        {
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room).ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<int>> GetBookedSeatIdsAsync(Guid showtimeId)
        {
            return await _context.BookingSeats
                .Include(bs => bs.Booking)
                .Where(bs => bs.Booking.ShowtimeId == showtimeId
                          && (bs.Status == "held" || bs.Status == "confirmed")
                          && (bs.Booking.Status == "pending" || bs.Booking.Status == "confirmed"))
                .Select(bs => bs.SeatId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UpcomingShowtime>> GetShowtimesByDateAsync(DateTime date, Guid? movieId = null, int? cinemaId = null)
        {
            var query = _context.UpcomingShowtimes.AsQueryable();
            var targetDate = DateOnly.FromDateTime(date);
            query = query.Where(s => DateOnly.FromDateTime(s.StartTime) == targetDate);

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            // Lọc theo rạp
            if (cinemaId.HasValue)
            {
                query = query.Where(s => s.CinemaId == cinemaId.Value);
            }

            return await query.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(int roomId, DateTime startTime, DateTime endTime)
        {
            return await _context.Showtimes
                .Where(s => s.RoomId == roomId && !s.IsCancelled)
                .AnyAsync(s => startTime < s.EndTime && endTime > s.StartTime);
        }

        public async Task<Showtime> AddShowTimeAsync(Showtime showtime)
        {
            await _context.Showtimes.AddAsync(showtime);
            return showtime;
        }

        public void UpShowTimedate(Showtime showtime)
        {
            _context.Showtimes.Update(showtime);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}