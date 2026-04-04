using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class PointTransaction
{
    public Guid Id { get; set; }

    public Guid MembershipId { get; set; }

    public Guid? BookingId { get; set; }

    public int Points { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual MembershipCard Membership { get; set; } = null!;
}
