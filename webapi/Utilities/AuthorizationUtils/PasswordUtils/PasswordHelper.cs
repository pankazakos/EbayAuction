using System;
using System.Security.Cryptography;
using System.Text;

namespace webapi.Utilities.AuthorizationUtils.PasswordUtils
{
    public class PasswordHelper
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32]; // 256 bits
            using (var provider = RandomNumberGenerator.Create())
            {
                provider.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{salt}{password}";
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);

                byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            string newHash = HashPassword(enteredPassword, storedSalt);
            return newHash == storedHash;
        }

    }
}
