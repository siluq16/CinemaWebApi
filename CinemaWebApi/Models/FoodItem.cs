using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class FoodItem
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal BasePrice { get; set; }

    public int? Calories { get; set; }

    public bool IsCombo { get; set; }

    public bool IsAvailable { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual FoodCategory Category { get; set; } = null!;

    public virtual ICollection<FoodComboItem> FoodComboItemCombos { get; set; } = new List<FoodComboItem>();

    public virtual ICollection<FoodComboItem> FoodComboItemItems { get; set; } = new List<FoodComboItem>();

    public virtual ICollection<FoodOrderItem> FoodOrderItems { get; set; } = new List<FoodOrderItem>();
}
