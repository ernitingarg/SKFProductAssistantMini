using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SKFProductAssistant.Function.Products
{
    public class Product
    {
        /// <summary>
        /// Single list containing all attributes from different sections.
        /// </summary>
        public HashSet<AttributeDetail> Attributes { get; } = new();

        [JsonExtensionData] Dictionary<string, JToken> _attributeSections = new();

        /// <summary>
        /// Flatten all attributes from different sections.
        /// </summary>
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            foreach (var section in _attributeSections.Values)
            {
                if (section.Type == JTokenType.Array &&
                    section.First?.Type == JTokenType.Object)
                {
                    var attributes = section.ToObject<List<AttributeDetail>>();
                    foreach (var attribute in attributes)
                    {
                        Attributes.Add(attribute);
                    }
                }
            }
        }
    }

    public class AttributeDetail
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Value}{Unit}".ToLower();
        }
    }
}
