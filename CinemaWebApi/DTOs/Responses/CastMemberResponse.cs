namespace CinemaWebApi.DTOs.Responses
{
    public class CastMemberResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Nationality { get; set; }
        public DateOnly? BirthDate { get; set; }
    }
}