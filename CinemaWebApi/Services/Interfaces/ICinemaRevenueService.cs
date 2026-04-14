using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;

namespace CinemaWebApi.Services.Interfaces
{
    public interface ICinemaRevenueService
    {
        Task<IEnumerable<CinemaRevenue>> GetCinemaRevenuesAsync();
        Task<IEnumerable<MonthlyRevenueResponses>> GetMonthlyRevenueAsync(int year);
        Task<IEnumerable<WeeklyRevenueResponses>> GetWeeklyRevenueAsync();
    }
}
