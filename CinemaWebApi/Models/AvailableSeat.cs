using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class AvailableSeat
{
    public int SeatId { get; set; }

    public int RoomId { get; set; }

    public string RowLabel { get; set; } = null!;

    public int SeatNumber { get; set; }

    public string SeatType { get; set; } = null!;

    public int? XPosition { get; set; }

    public int? YPosition { get; set; }
}
