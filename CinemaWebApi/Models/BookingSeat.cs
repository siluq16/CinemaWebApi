using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class BookingSeat
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public int SeatId { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual SeatLayout Seat { get; set; } = null!;
}
