using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class CinemaRevenueService : ICinemaRevenueService
    {
        private readonly ICinemaRevenueRepository _revenueRepo;
        private readonly IBookingRepository _bookingRepo;

        public CinemaRevenueService(ICinemaRevenueRepository repository, IBookingRepository bookingRepo)
        {
            _revenueRepo = repository;
            _bookingRepo = bookingRepo;
        }

        public async Task<IEnumerable<CinemaRevenue>> GetCinemaRevenuesAsync()
        {
            return await _revenueRepo.GetAllCinemaRevenueAsync();
        }

        public async Task<IEnumerable<MonthlyRevenueResponses>> GetMonthlyRevenueAsync(int year)
        {
            var bookings = await _bookingRepo.GetConfirmedBookingsByYearAsync(year);
            // ... logic tính toán trả về MonthlyRevenueDto như đã hướng dẫn
            var groupedData = bookings
            .GroupBy(b => b.CreatedAt.Month)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.FinalAmount));

            var result = new List<MonthlyRevenueResponses>();

            // Tạo mảng 12 tháng, nếu tháng nào không có DB trả về thì set = 0
            for (int i = 1; i <= 12; i++)
            {
                result.Add(new MonthlyRevenueResponses
                {
                    Month = $"T{i}",
                    Revenue = groupedData.ContainsKey(i) ? groupedData[i] : 0
                });
            }

            return result;
        }

        public async Task<IEnumerable<WeeklyRevenueResponses>> GetWeeklyRevenueAsync()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-6).Date;

            // Đổi sang gọi hàm truy vấn theo khoảng thời gian
            var bookings = await _bookingRepo.GetConfirmedBookingsByDateRangeAsync(startDate, endDate);

            var groupedData = bookings
                .GroupBy(b => b.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Sum(b => b.FinalAmount));

            var result = new List<WeeklyRevenueResponses>();

            for (int i = 0; i <= 6; i++)
            {
                DateTime currentDate = startDate.AddDays(i);
                string dayOfWeek = currentDate.DayOfWeek == DayOfWeek.Sunday
                    ? "CN"
                    : $"T{(int)currentDate.DayOfWeek + 1}";

                result.Add(new WeeklyRevenueResponses
                {
                    Day = dayOfWeek,
                    Value = groupedData.ContainsKey(currentDate.Date) ? groupedData[currentDate.Date] : 0
                });
            }

            return result;
        }
    }
}
