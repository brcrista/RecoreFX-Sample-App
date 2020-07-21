using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileSync.Service.Models
{
    /// <summary>
    /// Manages a collection of hyperlinks according to the JSON Hypertext Application Language (HAL).
    /// </summary>
    /// <remarks>
    /// The HAL specification only exists as an RFC draft (now expired): https://tools.ietf.org/html/draft-kelly-json-hal-08.
    /// Note that there's no built-in way in ASP.NET Core to change the JSON property name of a type member in its JSON serialization,
    /// so a <see cref="HAL" /> property should be named <c>_links</c>.
    /// </remarks>
    public sealed class HAL : Dictionary<string, HAL.Link>
    {
        public sealed class Link
        {
            public Link(Uri href)
            {
                Href = href;
            }

            [Required, Url] // TODO what happens without this?
            public Uri Href { get; set; }

            public bool? Templated { get; set; }

            public string? Type { get; set; }

            public string? Deprecation { get; set; }

            public string? Name { get; set; }
        }

        public HAL(Uri self)
        {
            Add("self", new Link(self));
        }

        public HAL(Uri self, IReadOnlyDictionary<string, Uri> links)
            : this(self)
        {
            foreach (var link in links)
            {
                Add(link.Key, new Link(link.Value));
            }
        }
    }
}
