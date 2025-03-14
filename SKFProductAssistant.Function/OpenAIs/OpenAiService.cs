using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Polly;
using Polly.Retry;
using SKFProductAssistant.Function.Caches;
using SKFProductAssistant.Function.Configs;
using SKFProductAssistant.Function.Utils;
using System;
using System.ClientModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace SKFProductAssistant.Function.OpenAIs
{
    /// <summary>
    /// Implementation of OpenAI service using external
    /// OpenAI API (eg: Azure OpenAI)
    /// </summary>
    public class OpenAiService : IOpenAiService
    {
        readonly ChatClient _chatClient;
        readonly IDistributedCacheService _cacheService;
        readonly ILogger<OpenAiService> _logger;

        const string SystemMessage =
            "You are an assistant that extracts SKF product names from user queries.";

        public OpenAiService(
            IDistributedCacheService cacheService,
            OpenAiConfig openAiConfig,
            ILogger<OpenAiService> logger)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            openAiConfig = openAiConfig ?? throw new ArgumentNullException(nameof(openAiConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var azureOpenAiClient = new AzureOpenAIClient(new Uri(openAiConfig.Endpoint),
                new ApiKeyCredential(openAiConfig.ApiKey));
            _chatClient = azureOpenAiClient.GetChatClient(openAiConfig.Model);
        }

        /// <inheritdoc cref="IOpenAiService.ExtractProductNameAsync"/>/>
        public async Task<string> ExtractProductNameAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Received an empty user query.");
                return string.Empty;
            }

            try
            {
                _logger.LogInformation("Extracting product name for the user query.");

                var hashedKey = HashUtils.HashData($"OPENAPI_{QueryUtils.NormalizeQuery(query)}");

                // Retrieve product name from the cache
                string result = await _cacheService.GetAsync<string>(hashedKey);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }

                var chatCompletion = await GetRetryPolicy().ExecuteAsync(
                    () => GetChatCompletionAsync(query));
                string productName = chatCompletion?.Content[0]?.Text?.Trim();
                if (string.IsNullOrEmpty(productName))
                {
                    _logger.LogInformation("No product name found by OpenAI for the user query.");
                    return string.Empty;
                }

                _logger.LogInformation($"Extracted product name by OpenAI: {productName}.");

                // Set the product name to the cache
                await _cacheService.SetAsync(hashedKey, productName);

                return productName;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while extracting product name using OpenAI.");
                return string.Empty;
            }
        }

        AsyncRetryPolicy<ChatCompletion> GetRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<ChatCompletion>(result => result == null)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)),
                    (_, timeSpan, retryCount, _) =>
                    {
                        _logger.LogWarning(
                            "Retrying OpenAI request. " +
                            "Attempt: [{RetryCount}], Waiting: [{Delay}s]...",
                            retryCount,
                            timeSpan.TotalSeconds
                        );
                    });
        }

        public async Task<ChatCompletion> GetChatCompletionAsync(string query)
        {
            string prompt = $"Extract only the SKF product name from this query: \"{query}\".";

            ChatMessage[] messages =
            [
                new SystemChatMessage(SystemMessage),
                new UserChatMessage(prompt)
            ];

            return await _chatClient.CompleteChatAsync(messages);
        }
    }
}
