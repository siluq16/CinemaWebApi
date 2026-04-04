using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class UserPromotionUsage
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PromotionId { get; set; }

    public Guid BookingId { get; set; }

    public DateTime UsedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
