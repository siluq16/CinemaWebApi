using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class CinemaFoodMenu
{
    public int Id { get; set; }

    public int CinemaId { get; set; }

    public int FoodItemId { get; set; }

    public bool IsAvailable { get; set; }

    public decimal? CustomPrice { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual FoodItem FoodItem { get; set; } = null!;
}
