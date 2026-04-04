using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class FoodComboItem
{
    public int Id { get; set; }

    public int ComboId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public virtual FoodItem Combo { get; set; } = null!;

    public virtual FoodItem Item { get; set; } = null!;
}
