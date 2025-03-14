using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SKFProductAssistant.Function.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SKFProductAssistant.Function.Products
{
    public class ProductService : IProductService
    {
        readonly ILogger<ProductService> _logger;

        public ProductService(
            ILogger<ProductService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc cref="IProductService.GetProductAttributeDetailAsync"/>
        public async Task<AttributeDetail> GetProductAttributeDetailAsync(
            string productName,
            string query)
        {
            if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Invalid request: Product name or query is empty.");
                return null;
            }

            _logger.LogInformation("Fetching all attributes for the product.");
            var attributes = await LoadAllAttributeDetailsAsync(productName);
            if (attributes == null || attributes.Count == 0)
            {
                _logger.LogInformation("No attributes found for the product.");
                return null;
            }

            var matchedAttribute = QueryUtils.ExtractBestMatchingAttribute(
                productName,
                query,
                attributes);
            if (matchedAttribute == null)
            {
                _logger.LogInformation("No best matching attribute found for the product.");
                return null;
            }

            _logger.LogInformation("Found best matching attribute for the product.");
            return matchedAttribute;
        }

        async Task<HashSet<AttributeDetail>> LoadAllAttributeDetailsAsync(
            string productName)
        {
            string basePath = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot")
                              ?? Directory.GetCurrentDirectory();
            string filePath = Path.Combine(basePath, "Products", "datasheets", $"{productName}.json");
            if (!File.Exists(filePath))
            {
                _logger.LogInformation($"Datasheet not found for product: {productName}");
                return null;
            }

            try
            {
                _logger.LogDebug($"Reading datasheet for product: {productName}");
                string jsonString = await File.ReadAllTextAsync(filePath);
                var product = JsonConvert.DeserializeObject<Product>(jsonString);

                return product.Attributes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error loading datasheet for product: {productName}");
                return null;
            }
        }
    }
}
