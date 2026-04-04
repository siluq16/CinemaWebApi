using System.Security.Cryptography;
using System.Text;

namespace CinemaWebApi.Helpers
{
    public static class SecurityHelper
    {
        public static string GenerateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
        }

        public static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}