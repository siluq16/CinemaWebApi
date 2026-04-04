using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<BookingResponse?> AddFoodToBookingAsync(Guid bookingId, List<BookingFoodItemRequest> foodRequests);
        Task<BookingResponse?> ApplyPromotionAsync(Guid bookingId, Guid userId, string promotionCode);
        Task<BookingResponse?> GetBookingByIdAsync(Guid id);
        Task CancelExpiredBookingsAsync();
    }
}