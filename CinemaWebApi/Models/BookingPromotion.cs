using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class BookingPromotion
{
    public Guid BookingId { get; set; }

    public Guid PromotionId { get; set; }

    public decimal AppliedDiscount { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Promotion Promotion { get; set; } = null!;
}
