using SKFProductAssistant.Function.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SKFProductAssistant.Function.Utils
{
    /// <summary>
    /// Utility for query normalization,
    /// and extracting attribute details.
    /// </summary>
    public static class QueryUtils
    {
        // Precompiled regex for stop words
        static readonly Regex StopWordsRegex = new(
            @"\b(?:what|is|\'s|the|of|for|from|in|to|a|an|about|tell|me|this|give|value)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);


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

        /// <summary>
        /// Extracts the best matching attribute detail from the provided query.
        /// </summary>
        /// <param name="productName">
        /// The name of the product.
        /// </param>
        /// <param name="query">
        /// The user's query.
        /// </param>
        /// <param name="attributes">
        /// A set of product attributes to match against.
        /// </param>
        /// <returns>
        /// The best matching attribute's details or null if no match is found.
        /// </returns>
        public static AttributeDetail ExtractBestMatchingAttribute(
            string productName,
            string query,
            HashSet<AttributeDetail> attributes)
        {
            var normalizedQuery = NormalizeQuery(query, false, false);

            // remove product name
            normalizedQuery = normalizedQuery.Replace(
                productName,
                "",
                StringComparison.InvariantCultureIgnoreCase);

            return FindBestMatchingAttribute(normalizedQuery, attributes);
        }

        /// <summary>
        /// Finds the best matching attribute from a given query.
        /// </summary>
        static AttributeDetail FindBestMatchingAttribute(
            string query,
            HashSet<AttributeDetail> attributes)
        {
            var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var candidateAttributes = attributes
                .Where(attr => queryWords.Any(word => attr.Name.Contains(
                    word, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (!candidateAttributes.Any())
            {
                return null;
            }

            var bestMatchAttribute = candidateAttributes
                .OrderByDescending(attr => queryWords.Count(
                    word => attr.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault();

            return bestMatchAttribute;
        }
    }
}