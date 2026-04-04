using System;
using System.Collections.Generic;

namespace CinemaWebApi.Models;

public partial class Movie
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? OriginalTitle { get; set; }

    public string? Description { get; set; }

    public int DurationMinutes { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? BannerUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public string Language { get; set; } = null!;

    public string? Country { get; set; }

    public string Status { get; set; } = null!;

    public decimal? AvgRating { get; set; }

    public int ReviewCount { get; set; }

    public int AgeRating { get; set; }

    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
