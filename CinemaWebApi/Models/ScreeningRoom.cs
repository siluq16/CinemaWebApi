using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class ScreeningRoom
{
    public int Id { get; set; }

    public int CinemaId { get; set; }

    public string Name { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public int TotalSeats { get; set; }

    public bool IsActive { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual ICollection<SeatLayout> SeatLayouts { get; set; } = new List<SeatLayout>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
