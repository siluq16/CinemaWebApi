namespace CinemaWebApi.DTOs.Responses
{
    public class PointTransactionResponse
    {
        public Guid Id { get; set; }
        public int Points { get; set; } // +5000 hoặc -2000
        public string Reason { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class MembershipCardResponse
    {
        public string CardNumber { get; set; } = null!;
        public string Tier { get; set; } = null!; // silver, gold...
        public int TotalPoints { get; set; }
        public int UsedPoints { get; set; }
        public int CurrentPoints => TotalPoints - UsedPoints; // Điểm còn lại có thể xài
        public List<PointTransactionResponse> RecentTransactions { get; set; } = new();
    }
}