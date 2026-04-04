using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<GenreResponse>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return genres.Select(g => new GenreResponse { Id = g.Id, Name = g.Name, Slug = g.Slug });
        }

        public async Task<GenreResponse?> GetGenreByIdAsync(int id)
        {
            var g = await _genreRepository.GetByIdAsync(id);
            if (g == null) return null;
            return new GenreResponse { Id = g.Id, Name = g.Name, Slug = g.Slug };
        }

        public async Task<GenreResponse> CreateGenreAsync(GenreRequest request)
        {
            var genre = new Genre { Name = request.Name, Slug = request.Slug };
            await _genreRepository.AddAsync(genre);
            await _genreRepository.SaveChangesAsync();
            return new GenreResponse { Id = genre.Id, Name = genre.Name, Slug = genre.Slug };
        }

        public async Task<GenreResponse?> UpdateGenreAsync(int id, GenreRequest request)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return null;

            genre.Name = request.Name;
            genre.Slug = request.Slug;

            _genreRepository.Update(genre);
            await _genreRepository.SaveChangesAsync();

            return new GenreResponse { Id = genre.Id, Name = genre.Name, Slug = genre.Slug };
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return false;

            _genreRepository.Delete(genre);
            await _genreRepository.SaveChangesAsync();
            return true;
        }
    }
}