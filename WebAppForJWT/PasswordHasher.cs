using System.Security.Cryptography;
using System.Text;

namespace WebAppForJWT
{
    public static class PasswordHasher
    {
        public static string HashPassword(string enteredPassword)
        {
            using (var HashP = SHA256.Create())
            {
                var hashedBytes = HashP.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
    }
}
