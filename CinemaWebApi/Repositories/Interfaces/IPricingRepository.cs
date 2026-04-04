using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IPricingRepository
    {
        Task<List<PricingRule>> GetActiveRulesAsync();
        Task<bool> IsHolidayAsync(DateTime date);
        // --- CRUD PRICING RULE ---
        Task<IEnumerable<PricingRule>> GetAllRulesAsync();
        Task<PricingRule?> GetRuleByIdAsync(int id);
        Task<PricingRule> AddRuleAsync(PricingRule rule);
        void UpdateRule(PricingRule rule);

        // --- CRUD HOLIDAY ---
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task<Holiday?> GetHolidayByIdAsync(int id);
        Task<Holiday> AddHolidayAsync(Holiday holiday);
        void UpdateHoliday(Holiday holiday);
        void DeleteHoliday(Holiday holiday);

        Task<bool> SaveChangesAsync();
    }
}