using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User> AddAsync(User user);
        Task AddPasswordResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidResetTokenAsync(string tokenHash);
        void UpdatePasswordResetToken(PasswordResetToken token);
        void UpdateUser(User user);

        Task<OauthAccount?> GetOAuthAccountAsync(string provider, string providerUserId);
        Task CreateUserWithOAuthAsync(User user, OauthAccount oauthAccount);
        Task AddOAuthAccountAsync(OauthAccount oauthAccount);
        Task<bool> SaveChangesAsync();
    }
}