using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class PricingService : IPricingService
    {
        private readonly IPricingRepository _pricingRepo;

        public PricingService(IPricingRepository pricingRepo)
        {
            _pricingRepo = pricingRepo;
        }

        // ==========================================
        // 1. QUẢN LÝ LUẬT GIÁ (PRICING RULES)
        // ==========================================
        public async Task<IEnumerable<PricingRule>> GetAllRulesAsync()
        {
            return await _pricingRepo.GetAllRulesAsync();
        }

        public async Task<PricingRule> CreateRuleAsync(PricingRuleRequest request)
        {
            var rule = new PricingRule
            {
                RuleName = request.RuleName,
                SeatType = request.SeatType,
                ScreenFormat = request.ScreenFormat,
                StartTimeFilter = request.StartTimeFilter,
                EndTimeFilter = request.EndTimeFilter,
                DayOfWeekFilter = request.DayOfWeekFilter,
                IsHoliday = request.IsHoliday,
                BasePrice = request.BasePrice,
                Priority = request.Priority,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _pricingRepo.AddRuleAsync(rule);
            await _pricingRepo.SaveChangesAsync();
            return rule;
        }

        public async Task<PricingRule?> UpdateRuleAsync(int id, PricingRuleRequest request)
        {
            var rule = await _pricingRepo.GetRuleByIdAsync(id);
            if (rule == null) return null;

            rule.RuleName = request.RuleName;
            rule.SeatType = request.SeatType;
            rule.ScreenFormat = request.ScreenFormat;
            rule.StartTimeFilter = request.StartTimeFilter;
            rule.EndTimeFilter = request.EndTimeFilter;
            rule.DayOfWeekFilter = request.DayOfWeekFilter;
            rule.IsHoliday = request.IsHoliday;
            rule.BasePrice = request.BasePrice;
            rule.Priority = request.Priority;

            _pricingRepo.UpdateRule(rule);
            await _pricingRepo.SaveChangesAsync();
            return rule;
        }

        public async Task<bool> ToggleRuleStatusAsync(int id)
        {
            var rule = await _pricingRepo.GetRuleByIdAsync(id);
            if (rule == null) return false;

            rule.IsActive = !rule.IsActive; // Đảo ngược trạng thái (Bật thành Tắt, Tắt thành Bật)
            _pricingRepo.UpdateRule(rule);
            await _pricingRepo.SaveChangesAsync();
            return true;
        }

        // ==========================================
        // 2. QUẢN LÝ NGÀY LỄ (HOLIDAYS)
        // ==========================================
        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            return await _pricingRepo.GetAllHolidaysAsync();
        }

        public async Task<Holiday> CreateHolidayAsync(HolidayRequest request)
        {
            var holiday = new Holiday
            {
                HolidayDate = request.HolidayDate,
                Description = request.Description
            };

            await _pricingRepo.AddHolidayAsync(holiday);
            await _pricingRepo.SaveChangesAsync();
            return holiday;
        }

        public async Task<Holiday?> UpdateHolidayAsync(int id, HolidayRequest request)
        {
            var holiday = await _pricingRepo.GetHolidayByIdAsync(id);
            if (holiday == null) return null;

            holiday.HolidayDate = request.HolidayDate;
            holiday.Description = request.Description;

            _pricingRepo.UpdateHoliday(holiday);
            await _pricingRepo.SaveChangesAsync();
            return holiday;
        }

        public async Task<bool> DeleteHolidayAsync(int id)
        {
            var holiday = await _pricingRepo.GetHolidayByIdAsync(id);
            if (holiday == null) return false;

            _pricingRepo.DeleteHoliday(holiday);
            await _pricingRepo.SaveChangesAsync();
            return true;
        }
    }
}