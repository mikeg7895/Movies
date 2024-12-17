using System.Security.Cryptography;
using System.Text;

namespace Movies.Services
{
    public class PasswordEncrypterService : IPasswordEncrypter
    {
        public string HashPassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = SHA256.HashData(passwordBytes);

            return Convert.ToBase64String(hashBytes);
        }

        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
