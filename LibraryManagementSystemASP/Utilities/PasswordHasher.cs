// Utilities/PasswordHasher.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagementSystemASP.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Handle null or empty password
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes and compute the hash
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    sb.Append(b.ToString("x2")); // Format as hexadecimal
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Handle null cases
            if (inputPassword == null || storedPassword == null)
            {
                return false;
            }

            // Hash the input password
            string hashedInputPassword = HashPassword(inputPassword);

            // Check if the hashed input matches the stored password
            return hashedInputPassword.Equals(storedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}