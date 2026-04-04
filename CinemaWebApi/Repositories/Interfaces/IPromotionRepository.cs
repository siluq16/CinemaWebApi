using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IPromotionRepository

    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<bool> CodeExistsAsync(string code);

        Task<Promotion> AddAsync(Promotion promotion);

        Task<Promotion?> GetByCodeAsync(string code);

        Task<Promotion?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsExceptIdAsync(string code, Guid excludeId);

        void Update(Promotion promotion);

        Task<int> GetUserUsageCountAsync(Guid userId, Guid promotionId);
        Task<bool> HasBookingAppliedPromotionAsync(Guid bookingId);
        Task AddBookingPromotionAsync(BookingPromotion bookingPromotion);
        Task AddUserPromotionUsageAsync(UserPromotionUsage usage);

        Task RevertPromotionUsageAsync(Guid bookingId);
        Task<bool> SaveChangesAsync();
    }
}