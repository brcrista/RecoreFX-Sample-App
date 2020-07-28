using System;
using System.Text.Json.Serialization;

namespace FileSync.Common.ApiModels
{
    public sealed class File
    {
        public Filepath Path { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        [JsonPropertyName("_links")]
        public HAL Links { get; set; }
    }
}
