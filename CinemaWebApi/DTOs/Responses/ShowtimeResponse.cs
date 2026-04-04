namespace CinemaWebApi.DTOs.Responses
{
    public class ShowtimeSeatResponse
    {
        public int SeatId { get; set; }
        public string RowLabel { get; set; } = null!;
        public int SeatNumber { get; set; }
        public string SeatType { get; set; } = null!;
        public int? XPosition { get; set; }
        public int? YPosition { get; set; }

        public decimal Price { get; set; }

        public bool IsBooked { get; set; }
    }

    public class ShowtimeResponse
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; } = null!;
        public int DurationMinutes { get; set; }

        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public string CinemaName { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Language { get; set; } = null!;
        public string SubtitleType { get; set; } = null!;
        public string ScreenFormat { get; set; } = null!;
        public bool IsCancelled { get; set; }
        public string? Notes { get; set; }

    }
}