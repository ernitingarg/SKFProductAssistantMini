using Newtonsoft.Json;

namespace SKFProductAssistant.Function.Codecs
{
    /// <summary>
    /// JSON-based codec implementation for encoding and decoding objects.
    /// </summary>
    public class JsonCodec : ICodec
    {
        /// <inheritdoc cref="ICodec.Encode"/>
        public string Encode(object obj)
        {
            return obj == null
                ? string.Empty
                : JsonConvert.SerializeObject(obj);
        }

        /// <inheritdoc cref="ICodec.Decode{T}"/>
        public T Decode<T>(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? default
                : JsonConvert.DeserializeObject<T>(value);
        }

        /// <inheritdoc cref="ICodec.TryDecode{T}"/>
        public bool TryDecode<T>(string value, out T result)
        {
            try
            {
                result = Decode<T>(value);
                return result != null;
            }
            catch (JsonException)
            {
                result = default;
                return false;
            }
        }
    }
}
