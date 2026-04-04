using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> AreSeatsAvailableAsync(Guid showtimeId, List<int> seatIds);

        Task<Booking> CreateBookingTransactionAsync(Booking booking, List<BookingSeat> bookingSeats);//, FoodOrder? foodOrder, List<FoodOrderItem>? foodOrderItems);
        Task<bool> AddFoodToBookingAsync(Guid bookingId, FoodOrder foodOrder, List<FoodOrderItem> foodOrderItems, decimal foodTotalAmount);

        Task<Booking?> GetBookingByIdAsync(Guid id);
        Task<Booking?> GetBookingWithSeatsAsync(Guid id);

        Task<Booking?> GetBookingForPaymentAsync(Guid id);

        Task<List<Booking>> GetExpiredPendingBookingsAsync();

        void Update(Booking booking);

        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();

        Task<bool> SaveChangesAsync();
    }
}