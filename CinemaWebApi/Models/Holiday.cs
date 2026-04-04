using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Holiday
{
    public int Id { get; set; }

    public DateOnly HolidayDate { get; set; }

    public string? Description { get; set; }
}
