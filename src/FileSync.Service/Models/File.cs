using System;
using System.Text.Json.Serialization;

using Recore;

namespace FileSync.Service.Models
{
    public class Path : Of<string>
    {
        public Path(string value) => Value = value;
    }

    public sealed class File
    {
        public File(Path path, DateTime lastWriteTimeUtc, HAL links)
        {
            Path = path;
            LastWriteTimeUtc = lastWriteTimeUtc;
            Links = links;
        }

        public Path Path { get; }

        public DateTime LastWriteTimeUtc { get; }

        [JsonPropertyName("_links")]
        public HAL Links { get; }
    }
}
