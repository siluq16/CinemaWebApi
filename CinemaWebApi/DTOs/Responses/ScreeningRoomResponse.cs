namespace CinemaWebApi.DTOs.Responses
{
    public class ScreeningRoomResponse
    {
        public int Id { get; set; }
        public int CinemaId { get; set; }
        public string CinemaName { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public string RoomType { get; set; } = null!;
        public int TotalSeats { get; set; }
        public bool IsActive { get; set; }
    }
}
