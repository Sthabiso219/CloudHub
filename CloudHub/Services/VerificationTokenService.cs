
using System;
using System.Security.Cryptography;
using System.Text;

namespace CloudHub.Services
{
    public static class VerificationTokenService
    {
        public static string GenerateToken()
        {
            byte[] bytes = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string HashToken(string token)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(token);
                byte[] hash = sha.ComputeHash(bytes);

                return BitConverter.ToString(hash)
                    .Replace("-", "")
                    .ToLowerInvariant();
            }
        }
    }
}
