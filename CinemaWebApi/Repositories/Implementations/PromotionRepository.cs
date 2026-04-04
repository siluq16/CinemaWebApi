using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly CinemaWebApiContext _context;

        public PromotionRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Promotions.AnyAsync(p => p.Code == code);
        }

        public async Task<Promotion> AddAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            return promotion;
        }
        public async Task<Promotion?> GetByIdAsync(Guid id)
        {
            return await _context.Promotions.FindAsync(id);
        }
        public async Task<bool> CodeExistsExceptIdAsync(string code, Guid excludeId)
        {
            return await _context.Promotions.AnyAsync(p => p.Code == code && p.Id != excludeId);
        }
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code);
        }

        public void Update(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
        }

        public async Task<int> GetUserUsageCountAsync(Guid userId, Guid promotionId)
        {
            return await _context.UserPromotionUsages
                .CountAsync(u => u.UserId == userId && u.PromotionId == promotionId);
        }

        public async Task<bool> HasBookingAppliedPromotionAsync(Guid bookingId)
        {
            return await _context.BookingPromotions.AnyAsync(bp => bp.BookingId == bookingId);
        }

        public async Task AddBookingPromotionAsync(BookingPromotion bookingPromotion)
        {
            await _context.BookingPromotions.AddAsync(bookingPromotion);
        }

        public async Task AddUserPromotionUsageAsync(UserPromotionUsage usage)
        {
            await _context.UserPromotionUsages.AddAsync(usage);
        }

        public async Task RevertPromotionUsageAsync(Guid bookingId)
        {
            var bookingPromo = await _context.BookingPromotions.FirstOrDefaultAsync(bp => bp.BookingId == bookingId);
            if (bookingPromo != null)
            {
                var promo = await _context.Promotions.FindAsync(bookingPromo.PromotionId);
                if (promo != null && promo.UsedCount > 0)
                {
                    promo.UsedCount -= 1; 
                    _context.Promotions.Update(promo);
                }
                _context.BookingPromotions.Remove(bookingPromo); 

                // Xóa lịch sử dùng mã của user
                var usage = await _context.UserPromotionUsages.FirstOrDefaultAsync(u => u.BookingId == bookingId);
                if (usage != null)
                {
                    _context.UserPromotionUsages.Remove(usage);
                }
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}