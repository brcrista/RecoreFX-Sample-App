using System;
using System.Collections.Generic;

namespace FileSync.Common.ApiModels
{
    /// <summary>
    /// Manages a collection of hyperlinks according to the JSON Hypertext Application Language (HAL).
    /// </summary>
    /// <remarks>
    /// The HAL specification only exists as an RFC draft (now expired): https://tools.ietf.org/html/draft-kelly-json-hal-08.
    /// </remarks>
    public sealed class HAL : Dictionary<string, HAL.Link>
    {
        public sealed class Link
        {
            public Uri Href { get; set; }

            public bool? Templated { get; set; }

            public string Type { get; set; }

            public string Deprecation { get; set; }

            public string Name { get; set; }
        }

        /// <summary>
        /// Create an instance of <see cref="HAL"/>.
        /// </summary>
        /// <remarks>
        /// A parameterless constructor is needed for JSON deserialization,
        /// but it's ok if it's private.
        /// </remarks>
        private HAL()
        {
        }

        public static HAL Create(Uri self, IReadOnlyDictionary<string, Uri> links = null)
        {
            var result = new HAL
            {
                { "self", new Link { Href = self } }
            };

            if (links != null)
            {
                foreach (var link in links)
                {
                    result.Add(link.Key, new Link { Href = link.Value });
                }
            }

            return result;
        }
    }
}
