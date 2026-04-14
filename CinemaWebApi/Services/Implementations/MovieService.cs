using CinemaWebApi.Data;
using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly CinemaWebApiContext _context; // Dùng để fetch Genres dễ dàng

        public MovieService(IMovieRepository movieRepository, CinemaWebApiContext context)
        {
            _movieRepository = movieRepository;
            _context = context;
        }

        private MovieResponse MapToResponse(Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                EndDate = movie.EndDate,
                PosterUrl = movie.PosterUrl,
                TrailerUrl = movie.TrailerUrl, // Ban nãy mình cũng thiếu cái này trong response
                Language = movie.Language,
                Status = movie.Status,
                AgeRating = movie.AgeRating,
                AvgRating = movie.AvgRating ?? 0,

                // Bổ sung 4 trường mới
                BannerUrl = movie.BannerUrl,
                Country = movie.Country,
                ReviewCount = movie.ReviewCount,
                IsFeatured = movie.IsFeatured,

                Genres = movie.Genres.Select(g => g.Name).ToList(),

                // Đoạn code map Crews (nếu bạn đã làm ở bước trước)
                Crews = movie.MovieCrews.OrderBy(mc => mc.DisplayOrder).Select(mc => new MovieCrewResponse
                {
                    Name = mc.CastMember != null ? mc.CastMember.Name : (mc.Director != null ? mc.Director.Name : "Unknown"),
                    CharacterName = mc.CharacterName,
                    RoleLabel = mc.RoleLabel,
                    PhotoUrl = mc.CastMember != null ? mc.CastMember.PhotoUrl : (mc.Director != null ? mc.Director.PhotoUrl : null)
                }).ToList()
            };
        }

        public async Task<IEnumerable<MovieResponse>> GetAllMoviesAsync()
        {
            await AutoUpdateMovieStatusesAsync();
            var movies = await _movieRepository.GetAllAsync();
            return movies.Select(MapToResponse);
        }

        public async Task<IEnumerable<MovieResponse>> GetNowShowingMoviesAsync()
        {
            var movies = await _movieRepository.GetNowShowingMoviesAsync();
            return movies.Select(MapToResponse);
        }

        public async Task<IEnumerable<MovieResponse>> GetComingSoonMoviesAsync()
        {
            var movies = await _movieRepository.GetComingSoonMoviesAsync();
            return movies.Select(MapToResponse);
        }

        public async Task<IEnumerable<MovieResponse>> GetFeaturedMoviesAsync()
        {
            var movies = await _movieRepository.GetFeaturedMoviesAsync();
            return movies.Select(MapToResponse);
        }

        public async Task<MovieResponse?> GetMovieByIdAsync(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return null;
            return MapToResponse(movie);
        }

        public async Task<MovieResponse> CreateMovieAsync(MovieRequest request)
        {
            var movie = new Movie
            {
                Title = request.Title,
                OriginalTitle = request.OriginalTitle,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                EndDate = request.EndDate,
                PosterUrl = request.PosterUrl,
                TrailerUrl = request.TrailerUrl,
                BannerUrl = request.BannerUrl,
                Country = request.Country,
                IsFeatured = request.IsFeatured,
                ReviewCount = 0,
                Language = request.Language,
                Status = request.Status,
                AgeRating = request.AgeRating,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Logic xử lý Nhiều - Nhiều: Tìm các Genre có trong DB dựa vào list ID người dùng truyền lên
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                var genres = await _context.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync();
                foreach (var genre in genres)
                {
                    movie.Genres.Add(genre);
                }
            }

            if (request.Crews != null && request.Crews.Any())
            {
                foreach (var crewReq in request.Crews)
                {
                    movie.MovieCrews.Add(new MovieCrew
                    {
                        DirectorId = crewReq.DirectorId,
                        CastMemberId = crewReq.CastMemberId,
                        CharacterName = crewReq.CharacterName,
                        RoleLabel = crewReq.RoleLabel,
                        DisplayOrder = crewReq.DisplayOrder
                    });
                }
            }

            await _movieRepository.AddAsync(movie);
            await _movieRepository.SaveChangesAsync();

            return MapToResponse(movie);
        }

        public async Task<MovieResponse?> UpdateMovieAsync(Guid id, MovieRequest request)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return null;

            movie.Title = request.Title;
            movie.OriginalTitle = request.OriginalTitle;
            movie.Description = request.Description;
            movie.DurationMinutes = request.DurationMinutes;
            movie.ReleaseDate = request.ReleaseDate;
            movie.EndDate = request.EndDate;
            movie.PosterUrl = request.PosterUrl;
            movie.TrailerUrl = request.TrailerUrl;
            movie.BannerUrl = request.BannerUrl;
            movie.Country = request.Country;
            movie.IsFeatured = request.IsFeatured;
            movie.Language = request.Language;
            movie.Status = request.Status;
            movie.AgeRating = request.AgeRating;
            movie.UpdatedAt = DateTime.Now;

            movie.Genres.Clear();
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                var genres = await _context.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToListAsync();
                foreach (var genre in genres)
                {
                    movie.Genres.Add(genre);
                }
            }

            _context.MovieCrews.RemoveRange(movie.MovieCrews);

            // 2. Thêm lại Crew mới
            if (request.Crews != null && request.Crews.Any())
            {
                foreach (var crewReq in request.Crews)
                {
                    movie.MovieCrews.Add(new MovieCrew
                    {
                        DirectorId = crewReq.DirectorId,
                        CastMemberId = crewReq.CastMemberId,
                        CharacterName = crewReq.CharacterName,
                        RoleLabel = crewReq.RoleLabel,
                        DisplayOrder = crewReq.DisplayOrder
                    });
                }
            }

            _movieRepository.Update(movie);
            await _movieRepository.SaveChangesAsync();

            return MapToResponse(movie);
        }

        public async Task<MovieResponse?> UpdateMovieRatingAsync(Guid movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null) return null;

            var reviews = await _context.Reviews
                .Where(r => r.MovieId == movieId && r.IsVisible)
                .ToListAsync();

            if (reviews.Any())
            {
                movie.ReviewCount = reviews.Count;

                movie.AvgRating = Math.Round((decimal)reviews.Average(r => r.Rating), 1);
            }
            else
            {
                movie.ReviewCount = 0;
                movie.AvgRating = 0;
            }

            await _context.SaveChangesAsync();
            return MapToResponse(movie);
        }
        public async Task<bool> DeleteMovieAsync(Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return false;

            movie.Status = "cancelled";
            _movieRepository.Update(movie);
            await _movieRepository.SaveChangesAsync();

            return true;
        }
        public async Task AutoUpdateMovieStatusesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var moviesToStart = await _context.Movies
                .Where(m => m.Status == "coming_soon" && m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today)
                .ToListAsync();

            foreach (var movie in moviesToStart)
            {
                movie.Status = "now_showing";
            }

            var moviesToEnd = await _context.Movies
                .Where(m => m.Status == "now_showing" && m.EndDate.HasValue && m.EndDate.Value < today)
                .ToListAsync();

            foreach (var movie in moviesToEnd)
            {
                movie.Status = "ended";
                movie.IsFeatured = false;
            }

            if (moviesToStart.Any() || moviesToEnd.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}