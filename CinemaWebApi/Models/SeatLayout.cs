using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class SeatLayout
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string RowLabel { get; set; } = null!;

    public int SeatNumber { get; set; }

    public string SeatType { get; set; } = null!;

    public int? XPosition { get; set; }

    public int? YPosition { get; set; }

    public bool IsActive { get; set; }

    public virtual BookingSeat? BookingSeat { get; set; }

    public virtual ScreeningRoom Room { get; set; } = null!;
}
