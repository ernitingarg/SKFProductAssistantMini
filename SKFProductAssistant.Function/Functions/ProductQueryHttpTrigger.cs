using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SKFProductAssistant.Function.Caches;
using SKFProductAssistant.Function.OpenAIs;
using SKFProductAssistant.Function.Products;
using SKFProductAssistant.Function.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SKFProductAssistant.Function.Functions
{
    public class ProductQueryHttpTrigger
    {
        readonly IProductService _productService;
        readonly IOpenAiService _openAiService;
        readonly IDistributedCacheService _cacheService;
        readonly ILogger<ProductQueryHttpTrigger> _logger;

        public ProductQueryHttpTrigger(
            IProductService productService,
            IOpenAiService openAiService,
            IDistributedCacheService cacheService,
            ILogger<ProductQueryHttpTrigger> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName(nameof(QueryProductAttribute))]
        public async Task<IActionResult> QueryProductAttribute(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "GET",
                "POST",
                Route = "query")]
            HttpRequest req,
            ILogger log)
        {
            _logger.LogDebug("Received an incoming request");

            try
            {
                // Read query from the incoming request.
                string query = await ReadQueryFromRequest(req);
                if (string.IsNullOrEmpty(query))
                {
                    return CreateResponse(false, "Query cannot be empty.");
                }

                // Retrieve result from the cache
                var hashedKey = HashUtils.HashData($"FUNC_{QueryUtils.NormalizeQuery(query)}");
                string cachedResult = await _cacheService.GetAsync<string>(hashedKey);
                if (!string.IsNullOrEmpty(cachedResult))
                {
                    return CreateResponse(true, cachedResult);
                }

                // Extract product name from query using cache or OpenAI
                string productName = await _openAiService.ExtractProductNameAsync(query);
                if (string.IsNullOrEmpty(productName))
                {
                    return CreateResponse(false, "I'm sorry, I can't find that information.");
                }

                // Retrieve product's attribute details from datasheet
                var attributeDetail = await _productService.GetProductAttributeDetailAsync(
                    productName,
                    query);
                if (attributeDetail == null)
                {
                    return CreateResponse(false, "I'm sorry, I can't find that information.");
                }

                var result = $"The {attributeDetail.Name.ToLower()} of {productName} is {attributeDetail}";

                // set result to the cache
                await _cacheService.SetAsync(hashedKey, result);

                return CreateResponse(true, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing product query.");
                return CreateResponse(false, "An internal server error occurred.");
            }
        }

        async Task<string> ReadQueryFromRequest(HttpRequest req)
        {
            try
            {
                return req.Method switch
                {
                    "GET" => req.Query["q"],
                    "POST" => await new StreamReader(req.Body).ReadToEndAsync(),
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        static ObjectResult CreateResponse(bool success, string message)
        {
            return new OkObjectResult(new Response(success, message));
        }
    }

    /// <summary>
    /// Generic API response model.
    /// </summary>
    public class Response
    {
        public bool Success { get; }
        public string Message { get; }

        public Response(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
