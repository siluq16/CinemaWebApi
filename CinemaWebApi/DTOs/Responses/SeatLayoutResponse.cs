namespace CinemaWebApi.DTOs.Responses
{
    public class SeatLayoutResponse
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RowLabel { get; set; } = null!;
        public int SeatNumber { get; set; }
        public string SeatType { get; set; } = null!;
        public int? XPosition { get; set; }
        public int? YPosition { get; set; }
        public bool IsActive { get; set; }
    }
}