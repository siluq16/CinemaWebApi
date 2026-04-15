using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CinemaWebApiContext _context;

        public UserRepository(CinemaWebApiContext context) { _context = context; }

        public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByPhoneAsync(string phone) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Cinema)
                .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
                .Include(b => b.FoodOrders)
                    .ThenInclude(fo => fo.FoodOrderItems)
                        .ThenInclude(foi => foi.FoodItem)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task AddPasswordResetTokenAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
        }

        public async Task<PasswordResetToken?> GetValidResetTokenAsync(string tokenHash)
        {
            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash
                                       && t.ExpiresAt > DateTime.Now
                                       && t.UsedAt == null);
        }

        public void UpdatePasswordResetToken(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Update(token);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<OauthAccount?> GetOAuthAccountAsync(string provider, string providerUserId)
        {
            return await _context.OauthAccounts
                .FirstOrDefaultAsync(o => o.Provider == provider && o.ProviderUserId == providerUserId);
        }

        public async Task CreateUserWithOAuthAsync(User user, OauthAccount oauthAccount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(); // Sinh ra user.Id

                oauthAccount.UserId = user.Id;
                await _context.OauthAccounts.AddAsync(oauthAccount);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddOAuthAccountAsync(OauthAccount oauthAccount)
        {
            await _context.OauthAccounts.AddAsync(oauthAccount);
        }

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}