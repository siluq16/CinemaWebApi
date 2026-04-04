using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly CinemaWebApiContext _context;

        public MembershipRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<MembershipCard?> GetByUserIdAsync(Guid userId)
        {
            return await _context.MembershipCards
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<MembershipCard> CreateCardAsync(MembershipCard card)
        {
            await _context.MembershipCards.AddAsync(card);
            return card;
        }

        public async Task AddTransactionAsync(PointTransaction transaction)
        {
            await _context.PointTransactions.AddAsync(transaction);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
