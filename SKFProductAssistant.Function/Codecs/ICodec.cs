namespace SKFProductAssistant.Function.Codecs
{
    /// <summary>
    /// Defines an interface for encoding and decoding objects.
    /// </summary>
    public interface ICodec
    {
        /// <summary>
        /// Encodes an object as a string for storage or transmission.
        /// </summary>
        /// <param name="obj">
        /// The object to encode.
        /// </param>
        /// <returns>
        /// A string representation of the encoded object.
        /// </returns>
        string Encode(object obj);

        /// <summary>
        /// Decodes a string value back into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to decode.
        /// </typeparam>
        /// <param name="value">
        /// The encoded string representation.
        /// </param>
        /// <returns>
        /// The deserialized object of type <typeparamref name="T"/>.
        /// </returns>
        T Decode<T>(string value);

        /// <summary>
        /// Attempts to decode a string into an object of specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to decode.
        /// </typeparam>
        /// <param name="value">
        /// The encoded string representation.
        /// </param>
        /// <param name="result">
        /// The output deserialized object if successful.
        /// </param>
        /// <returns>
        /// True if decoding was successful; otherwise, false.
        /// </returns>
        bool TryDecode<T>(string value, out T result);
    }
}
