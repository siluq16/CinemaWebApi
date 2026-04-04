using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class FoodOrderItem
{
    public Guid Id { get; set; }

    public Guid FoodOrderId { get; set; }

    public int FoodItemId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }

    public string? SpecialRequest { get; set; }

    public string Status { get; set; } = null!;

    public virtual FoodItem FoodItem { get; set; } = null!;

    public virtual FoodOrder FoodOrder { get; set; } = null!;
}
