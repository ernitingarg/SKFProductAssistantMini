using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SKFProductAssistant.Function.Utils
{
    /// <summary>
    /// Utility for query normalization, hashing,
    /// and extracting attribute details.
    /// </summary>
    public static class QueryHelper
    {
        // Precompiled regex for stop words
        static readonly Regex StopWordsRegex = new(
            @"\b(?:what|is|what's|the|of|for|from|in|to|a|an|about|tell|me|this|give|value)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Hashes the given query.
        /// </summary>
        /// <param name="query">
        /// The input query to hash.
        /// </param>
        /// <returns>
        /// A Base64-encoded SHA256 hash of the query.
        /// </returns>
        public static string HashQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            byte[] queryBytes = Encoding.UTF8.GetBytes(query);
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(queryBytes);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Normalizes a query by removing stop words, punctuation,
        /// whitespace, and converting it to lowercase.
        /// </summary>
        /// <param name="query">
        /// The input query to normalize.
        /// </param>
        /// <param name="removeWhiteSpace">
        /// True if whitespace should be removed; false to retain it.
        /// </param>
        /// <param name="toLowerCase">
        /// True if the query should be converted to lowercase.
        /// </param>
        /// <returns>
        /// The normalized query.
        /// </returns>
        public static string NormalizeQuery(
            string query,
            bool removeWhiteSpace = true,
            bool toLowerCase = true)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            string normalizedQuery = StopWordsRegex.Replace(query, "");

            normalizedQuery = new string(normalizedQuery
                .Where(c => !char.IsPunctuation(c) && (!removeWhiteSpace || !char.IsWhiteSpace(c)))
                .ToArray());

            normalizedQuery = toLowerCase ? normalizedQuery.ToLowerInvariant() : normalizedQuery;

            return normalizedQuery.Trim();
        }
}