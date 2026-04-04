using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class OauthAccount
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Provider { get; set; } = null!;

    public string ProviderUserId { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenExpiresAt { get; set; }

    public DateTime LinkedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
