using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBooking();
        Task<bool> AreSeatsAvailableAsync(Guid showtimeId, List<int> seatIds);

        Task<Booking> CreateBookingTransactionAsync(Booking booking, List<BookingSeat> bookingSeats);//, FoodOrder? foodOrder, List<FoodOrderItem>? foodOrderItems);
        Task<bool> AddFoodToBookingAsync(Guid bookingId, FoodOrder foodOrder, List<FoodOrderItem> foodOrderItems, decimal foodTotalAmount);

        Task<Booking?> GetBookingByIdAsync(Guid id);
        Task<Booking?> GetBookingWithSeatsAsync(Guid id);

        Task<Booking?> GetBookingForPaymentAsync(Guid id);

        Task<List<Booking>> GetExpiredPendingBookingsAsync();
        Task ClearUserPendingBookingAsync(Guid userId, Guid showtimeId);
        Task<List<Booking>> GetConfirmedBookingsByYearAsync(int year);
        Task<List<Booking>> GetConfirmedBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        void Update(Booking booking);

        void RemoveRange(IEnumerable<BookingSeat> bookingSeats);
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();

        Task<bool> SaveChangesAsync();
    }
}