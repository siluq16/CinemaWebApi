namespace CinemaWebApi.DTOs.Responses
{
    public class GenreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
    }
}