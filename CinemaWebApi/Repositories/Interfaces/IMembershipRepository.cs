using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IMembershipRepository
    {
        Task<MembershipCard?> GetByUserIdAsync(Guid userId);
        Task<MembershipCard> CreateCardAsync(MembershipCard card);
        Task AddTransactionAsync(PointTransaction transaction);
        Task<bool> SaveChangesAsync();
    }
}
