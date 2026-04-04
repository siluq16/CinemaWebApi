using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class FoodOrder
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public int CinemaId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? PickupCounter { get; set; }

    public string? PickupCode { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual ICollection<FoodOrderItem> FoodOrderItems { get; set; } = new List<FoodOrderItem>();
}
