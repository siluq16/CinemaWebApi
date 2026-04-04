using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class UpcomingShowtime
{
    public Guid ShowtimeId { get; set; }

    public Guid MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? PosterUrl { get; set; }

    public int DurationMinutes { get; set; }

    public int AgeRating { get; set; }

    public int CinemaId { get; set; }

    public string CinemaName { get; set; } = null!;

    public string City { get; set; } = null!;

    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Language { get; set; } = null!;

    public string SubtitleType { get; set; } = null!;

    public string ScreenFormat { get; set; } = null!;

    public int TotalSeats { get; set; }

    public int? SeatsTaken { get; set; }
}
