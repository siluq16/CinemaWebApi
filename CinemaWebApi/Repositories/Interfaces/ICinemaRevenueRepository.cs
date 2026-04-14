using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface ICinemaRevenueRepository
    {
        Task<IEnumerable<CinemaRevenue>> GetAllCinemaRevenueAsync();
    }
}
