using System.Threading.Tasks;

namespace SKFProductAssistant.Function.OpenAIs
{
    /// <summary>
    /// Interface for interacting with OpenAI's GPT model.
    /// </summary>
    public interface IOpenAiService
    {
        /// <summary>
        /// Extract a product name from the given user query.
        /// </summary>
        /// <param name="query">User input question.</param>
        /// <returns>Extracted product name.</returns>
        Task<string> ExtractProductNameAsync(string query);
    }
}
