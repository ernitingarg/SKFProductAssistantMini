using System;

namespace SKFProductAssistant.Function.Configs
{
    /// <summary>
    /// Configuration settings for OpenAI Service.
    /// </summary>
    public class OpenAiConfig
    {
        /// <summary>
        /// The API endpoint for OpenAI.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The API key for authenticating requests.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The API version for OpenAI requests.
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// The model to use for OpenAI API calls.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Validates the OpenAI configuration settings.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Endpoint))
            {
                throw new ArgumentException($"{nameof(Endpoint)} cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new ArgumentException($"{nameof(ApiKey)} cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(Model))
            {
                throw new ArgumentException($"{nameof(Model)} cannot be null or empty.");
            }
        }
    }
}