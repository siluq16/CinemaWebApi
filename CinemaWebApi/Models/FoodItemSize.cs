using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class FoodItemSize
{
    public int Id { get; set; }

    public int FoodItemId { get; set; }

    public string SizeLabel { get; set; } = null!;

    public decimal ExtraPrice { get; set; }

    public bool IsDefault { get; set; }

    public virtual FoodItem FoodItem { get; set; } = null!;

    public virtual ICollection<FoodOrderItem> FoodOrderItems { get; set; } = new List<FoodOrderItem>();
}
