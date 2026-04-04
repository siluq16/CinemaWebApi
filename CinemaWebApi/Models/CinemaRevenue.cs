using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class CinemaRevenue
{
    public int CinemaId { get; set; }

    public string CinemaName { get; set; } = null!;

    public string City { get; set; } = null!;

    public decimal? TotalRevenue { get; set; }

    public decimal? FoodRevenue { get; set; }

    public decimal? TicketRevenue { get; set; }

    public int? TotalBookings { get; set; }
}
