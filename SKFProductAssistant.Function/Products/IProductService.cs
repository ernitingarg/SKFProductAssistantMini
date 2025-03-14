using System.Threading.Tasks;

namespace SKFProductAssistant.Function.Products
{
    /// <summary>
    /// Defines an interface for retrieving product attributes
    /// from datasheets based on user queries.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves details of a specific product attribute
        /// based on the user's query.
        /// </summary>
        /// <param name="productName">
        /// Name of product (e.g., "6205", "6205 N").
        /// </param>
        /// <param name="query">
        /// The user's query (e.g., "What is the width of 6205?").
        /// </param>
        /// <returns>
       /// An <see cref="AttributeDetail"/> object containing the requested
       /// attribute details, or null if no matching attribute found.
        /// </returns>
        Task<AttributeDetail> GetProductAttributeDetailAsync(
            string productName,
            string query);
    }
}
