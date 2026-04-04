using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Review
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid MovieId { get; set; }

    public Guid? BookingId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool IsVerified { get; set; }

    public bool IsVisible { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
