using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface ICastMemberRepository
    {
        Task<IEnumerable<CastMember>> GetAllAsync();
        Task<CastMember?> GetByIdAsync(Guid id);
        Task<CastMember> AddAsync(CastMember castMember);
        void Update(CastMember castMember);
        void Delete(CastMember castMember);
        Task<bool> SaveChangesAsync();
    }
}