using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class ShowtimePrice
{
    public Guid Id { get; set; }

    public Guid ShowtimeId { get; set; }

    public string SeatType { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

    public virtual Showtime Showtime { get; set; } = null!;
}
