using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Promotion
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    public decimal MinOrderValue { get; set; }

    public bool AppliesToFood { get; set; }

    public int? MaxUses { get; set; }

    public int MaxUsesPerUser { get; set; }

    public int UsedCount { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BookingPromotion> BookingPromotions { get; set; } = new List<BookingPromotion>();

    public virtual ICollection<UserPromotionUsage> UserPromotionUsages { get; set; } = new List<UserPromotionUsage>();
}
