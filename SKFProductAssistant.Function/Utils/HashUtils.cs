using System;
using System.Security.Cryptography;
using System.Text;

namespace SKFProductAssistant.Function.Utils
{
    /// Utility for hashing
    public class HashUtils
    {
        /// <summary>
        /// Hashes the given data.
        /// </summary>
        /// <param name="data">
        /// The input data to hash.
        /// </param>
        /// <returns>
        /// A Base64-encoded SHA256 hash of the query.
        /// </returns>
        public static string HashData(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return string.Empty;
            }

            byte[] queryBytes = Encoding.UTF8.GetBytes(data);
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(queryBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
