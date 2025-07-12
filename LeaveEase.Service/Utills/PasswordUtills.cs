
using System.Text;
using System.Security.Cryptography;

namespace LeaveEase.Service.Utills
{
    public static class PasswordUtills
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public static bool VerifyPassword(string inputPassword, string storedHash) 
        {
            var password = HashPassword(inputPassword) == storedHash;
            if (password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


