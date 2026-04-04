using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class MovieCrew
{
    public Guid Id { get; set; }

    public Guid MovieId { get; set; }

    public Guid? DirectorId { get; set; }

    public Guid? CastMemberId { get; set; }

    public string? CharacterName { get; set; }

    public string? RoleLabel { get; set; }

    public int DisplayOrder { get; set; }

    public virtual CastMember? CastMember { get; set; }

    public virtual Director? Director { get; set; }

    public virtual Movie Movie { get; set; } = null!;
}
