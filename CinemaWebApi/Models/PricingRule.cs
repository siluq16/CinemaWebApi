using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class PricingRule
{
    public int Id { get; set; }

    public string RuleName { get; set; } = null!;

    public string? SeatType { get; set; }

    public string? ScreenFormat { get; set; }

    public TimeOnly? StartTimeFilter { get; set; }

    public TimeOnly? EndTimeFilter { get; set; }

    public string? DayOfWeekFilter { get; set; }

    public bool IsHoliday { get; set; }

    public decimal BasePrice { get; set; }

    public int Priority { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
