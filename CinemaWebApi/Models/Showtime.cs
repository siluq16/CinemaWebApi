using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Showtime
{
    public Guid Id { get; set; }

    public Guid MovieId { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Language { get; set; } = null!;

    public string SubtitleType { get; set; } = null!;

    public string ScreenFormat { get; set; } = null!;

    public bool IsCancelled { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;

    public virtual ScreeningRoom Room { get; set; } = null!;
}
