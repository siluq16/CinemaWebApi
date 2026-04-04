using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Models;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IPricingService
    {
        Task<IEnumerable<PricingRule>> GetAllRulesAsync();
        Task<PricingRule> CreateRuleAsync(PricingRuleRequest request);
        Task<PricingRule?> UpdateRuleAsync(int id, PricingRuleRequest request);
        Task<bool> ToggleRuleStatusAsync(int id); // Xóa mềm/Bật tắt luật giá

        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task<Holiday> CreateHolidayAsync(HolidayRequest request);
        Task<Holiday?> UpdateHolidayAsync(int id, HolidayRequest request);
        Task<bool> DeleteHolidayAsync(int id);
    }
}