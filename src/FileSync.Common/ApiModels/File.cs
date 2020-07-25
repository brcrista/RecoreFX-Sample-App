using System;
using System.IO;
using System.Text.Json.Serialization;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class File
    {
        public Filepath Path { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        [JsonPropertyName("_links")]
        public HAL Links { get; set; }

        public static File FromFileInfo(FileInfo fileInfo)
            => FromFileInfo(fileInfo,
                selfUri: Optional<AbsoluteUri>.Empty);

        public static File FromFileInfo(FileInfo fileInfo, Optional<AbsoluteUri> selfUri)
            => new File
            {
                Path = new Filepath(fileInfo.Name),
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                Links = selfUri.Switch(
                    uri => HAL.Create(uri),
                    () => null)
            };
    }
}
