using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class MembershipCard
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string Tier { get; set; } = null!;

    public int TotalPoints { get; set; }

    public int UsedPoints { get; set; }

    public DateOnly? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    public virtual User User { get; set; } = null!;
}
