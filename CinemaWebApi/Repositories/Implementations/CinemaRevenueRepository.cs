using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Google;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class CinemaRevenueRepository : ICinemaRevenueRepository
    {
        private readonly CinemaWebApiContext _context;

        public CinemaRevenueRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CinemaRevenue>> GetAllCinemaRevenueAsync()
        {
            return await _context.Set<CinemaRevenue>()
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}
