using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace CinemaWebApi.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly CinemaWebApiContext _context;

        public BookingRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBooking()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Cinema)
                .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
                .Include(b => b.FoodOrders).ThenInclude(fo => fo.FoodOrderItems).ThenInclude(foi => foi.FoodItem)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AreSeatsAvailableAsync(Guid showtimeId, List<int> seatIds)
        {
            var bookedSeatsCount = await _context.BookingSeats
                .Include(bs => bs.Booking)
                .Where(bs => bs.Booking.ShowtimeId == showtimeId
                          && seatIds.Contains(bs.SeatId)
                          && (bs.Status == "held" || bs.Status == "confirmed")
                          && (bs.Booking.Status == "pending" || bs.Booking.Status == "confirmed"))
                .CountAsync();

            return bookedSeatsCount == 0;
        }

        public async Task<Booking> CreateBookingTransactionAsync(Booking booking, List<BookingSeat> bookingSeats)//, FoodOrder? foodOrder, List<FoodOrderItem>? foodOrderItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync(); // Lưu nháp để lấy booking.Id

                foreach (var seat in bookingSeats)
                {
                    seat.BookingId = booking.Id;
                }
                await _context.BookingSeats.AddRangeAsync(bookingSeats);

                //if (foodOrder != null && foodOrderItems != null && foodOrderItems.Any())
                //{
                //    foodOrder.BookingId = booking.Id;
                //    await _context.FoodOrders.AddAsync(foodOrder);
                //    await _context.SaveChangesAsync(); // Lưu nháp để lấy foodOrder.Id

                //    foreach (var item in foodOrderItems)
                //    {
                //        item.FoodOrderId = foodOrder.Id;
                //    }
                //    await _context.FoodOrderItems.AddRangeAsync(foodOrderItems);
                //}

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return booking;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Ném lỗi văng ngược lên Service xử lý
            }
        }

        public async Task<bool> AddFoodToBookingAsync(Guid bookingId, FoodOrder foodOrder, List<FoodOrderItem> foodOrderItems, decimal foodTotalAmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking != null)
                {
                    booking.FoodAmount += foodTotalAmount;
                    booking.FinalAmount += foodTotalAmount;
                    booking.UpdatedAt = DateTime.Now;
                }

                foodOrder.BookingId = bookingId;
                await _context.FoodOrders.AddAsync(foodOrder);
                await _context.SaveChangesAsync();

                foreach (var item in foodOrderItems)
                {
                    item.FoodOrderId = foodOrder.Id;
                }
                await _context.FoodOrderItems.AddRangeAsync(foodOrderItems);
                await _context.SaveChangesAsync();


                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Cinema)
                .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
                .Include(b => b.FoodOrders).ThenInclude(fo => fo.FoodOrderItems).ThenInclude(foi => foi.FoodItem)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetBookingWithSeatsAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetBookingForPaymentAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.User) 
                .Include(b => b.Showtime).ThenInclude(s => s.Movie) // Lấy thông tin Phim
                .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Cinema) // Lấy thông tin Rạp
                .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat) // Lấy số ghế
                .Include(b => b.FoodOrders)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetExpiredPendingBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                .Include(b => b.FoodOrders).ThenInclude(fo => fo.FoodOrderItems)
                .Where(b => b.Status == "pending" && b.ExpiresAt <= DateTime.Now)
                .ToListAsync();
        }

        public async Task ClearUserPendingBookingAsync(Guid userId, Guid showtimeId)
        {
            var oldBookings = await _context.Bookings
                .Include(b => b.BookingSeats)
                .Include(b => b.FoodOrders).ThenInclude(f => f.FoodOrderItems)
                .Where(b => b.UserId == userId && b.ShowtimeId == showtimeId && b.Status == "pending")
                .ToListAsync();

            if (oldBookings.Any())
            {
                foreach (var b in oldBookings)
                {
                    _context.BookingSeats.RemoveRange(b.BookingSeats);
                    foreach (var f in b.FoodOrders)
                    {
                        _context.FoodOrderItems.RemoveRange(f.FoodOrderItems);
                    }
                    _context.FoodOrders.RemoveRange(b.FoodOrders);
                }
                _context.Bookings.RemoveRange(oldBookings);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Booking>> GetConfirmedBookingsByYearAsync(int year)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.Status == "confirmed" && b.CreatedAt.Year == year)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetConfirmedBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings 
                .AsNoTracking()
                .Where(b => b.Status == "confirmed" && b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                .ToListAsync();
        }

        public void Update(Booking booking)
        {
            _context.Update(booking);
        }

        public void RemoveRange(IEnumerable<BookingSeat> bookingSeats)
        {
            _context.BookingSeats.RemoveRange(bookingSeats);
        }
        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}