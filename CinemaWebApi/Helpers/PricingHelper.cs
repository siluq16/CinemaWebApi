using CinemaWebApi.Models;

namespace CinemaWebApi.Helpers
{
    public static class PricingHelper
    {
        // Khai báo là public static để gọi trực tiếp mà không cần khởi tạo (new)
        public static decimal CalculateSeatPrice(
            string seatType,
            string screenFormat,
            bool isHoliday,
            string dayOfWeek,
            TimeOnly timeOfDay,
            List<PricingRule> rules)
        {
            var orderedRules = rules.OrderByDescending(r => r.Priority).ToList();

            foreach (var rule in orderedRules)
            {
                if (rule.IsHoliday && !isHoliday) continue;
                if (!string.IsNullOrEmpty(rule.SeatType) && rule.SeatType != seatType) continue;
                if (!string.IsNullOrEmpty(rule.ScreenFormat) && rule.ScreenFormat != screenFormat) continue;
                if (rule.StartTimeFilter.HasValue && timeOfDay < rule.StartTimeFilter.Value) continue;
                if (rule.EndTimeFilter.HasValue && timeOfDay > rule.EndTimeFilter.Value) continue;

                if (!string.IsNullOrEmpty(rule.DayOfWeekFilter))
                {
                    var allowedDays = rule.DayOfWeekFilter.Split(',');
                    if (!allowedDays.Contains(dayOfWeek)) continue;
                }

                // Nếu vượt qua hết các bộ lọc -> Tìm thấy giá áp dụng
                return rule.BasePrice;
            }

            // Bắn ra lỗi nếu thiếu config
            throw new Exception($"Chưa thiết lập giá vé cho ghế {seatType} - Định dạng {screenFormat}");
        }
    }
}