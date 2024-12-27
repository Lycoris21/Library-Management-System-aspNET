// Utilities/PasswordHasher.cs
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystemASP.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            var hasher = new PasswordHasher<object>();
            return hasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(null, storedHashedPassword, enteredPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}