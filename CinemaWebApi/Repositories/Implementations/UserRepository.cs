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