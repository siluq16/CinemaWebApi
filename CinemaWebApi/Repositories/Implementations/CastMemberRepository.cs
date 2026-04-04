using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class CastMemberRepository : ICastMemberRepository
    {
        private readonly CinemaWebApiContext _context;

        public CastMemberRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CastMember>> GetAllAsync() => await _context.CastMembers.ToListAsync();

        public async Task<CastMember?> GetByIdAsync(Guid id) => await _context.CastMembers.FindAsync(id);

        public async Task<CastMember> AddAsync(CastMember castMember)
        {
            await _context.CastMembers.AddAsync(castMember);
            return castMember;
        }

        public void Update(CastMember castMember) => _context.CastMembers.Update(castMember);

        public void Delete(CastMember castMember) => _context.CastMembers.Remove(castMember);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}