using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class PricingRepository : IPricingRepository
    {
        private readonly CinemaWebApiContext _context;

        public PricingRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<List<PricingRule>> GetActiveRulesAsync()
        {
            return await _context.PricingRules
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.Priority)
                .ToListAsync();
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var targetDate = DateOnly.FromDateTime(date);

            return await _context.Holidays
                .AnyAsync(h => h.HolidayDate == targetDate);
        }

        // --- CRUD PRICING RULE ---
        public async Task<IEnumerable<PricingRule>> GetAllRulesAsync()
        {
            return await _context.PricingRules.OrderByDescending(r => r.Priority).ToListAsync();
        }

        public async Task<PricingRule?> GetRuleByIdAsync(int id)
        {
            return await _context.PricingRules.FindAsync(id);
        }

        public async Task<PricingRule> AddRuleAsync(PricingRule rule)
        {
            await _context.PricingRules.AddAsync(rule);
            return rule;
        }

        public void UpdateRule(PricingRule rule)
        {
            _context.PricingRules.Update(rule);
        }

        // --- CRUD HOLIDAY ---
        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            return await _context.Holidays.OrderByDescending(h => h.HolidayDate).ToListAsync();
        }

        public async Task<Holiday?> GetHolidayByIdAsync(int id)
        {
            return await _context.Holidays.FindAsync(id);
        }

        public async Task<Holiday> AddHolidayAsync(Holiday holiday)
        {
            await _context.Holidays.AddAsync(holiday);
            return holiday;
        }

        public void UpdateHoliday(Holiday holiday)
        {
            _context.Holidays.Update(holiday);
        }

        public void DeleteHoliday(Holiday holiday)
        {
            _context.Holidays.Remove(holiday);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}