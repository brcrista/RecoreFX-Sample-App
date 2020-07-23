using System;
using System.Text.Json.Serialization;

using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class File
    {
        public File(Filepath path, DateTime lastWriteTimeUtc, HAL links)
        {
            Path = path;
            LastWriteTimeUtc = lastWriteTimeUtc;
            Links = links;
        }

        public Filepath Path { get; }

        public DateTime LastWriteTimeUtc { get; }

        [JsonPropertyName("_links")]
        public HAL Links { get; }
    }
}
