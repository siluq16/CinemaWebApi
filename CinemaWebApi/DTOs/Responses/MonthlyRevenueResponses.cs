namespace CinemaWebApi.DTOs.Responses
{
    public class MonthlyRevenueResponses
    {
        public string Month { get; set; } = null!; // Trả về "T1", "T2"...
        public decimal Revenue { get; set; }
    }
    public class WeeklyRevenueResponses
    {
        public string Day { get; set; } = null!;   // Trả về "T2", "T3"...
        public decimal Value { get; set; }         // Trả về doanh thu
    }
}
