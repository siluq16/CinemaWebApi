using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public string? TransactionId { get; set; }

    public string Method { get; set; } = null!;

    public decimal Amount { get; set; }

    public int PointsUsed { get; set; }

    public string Status { get; set; } = null!;

    public string? GatewayResponse { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? RefundedAt { get; set; }

    public decimal? RefundAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
