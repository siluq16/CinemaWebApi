using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class CastMember
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Nationality { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
}
