using System.Security.Cryptography;
using System.Text;

namespace MemoryboardAPI.Services
{
    public class PasswordService
    {
        public byte[] GenerateSalt(int length = 16)
        {
            byte[] salt = new byte[length];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            return salt;
        }

        public string HashPassword(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combined = new byte[salt.Length + passwordBytes.Length];

            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

            byte[] hash = SHA256.HashData(combined);
            return Convert.ToBase64String(hash);
        }
    }
}