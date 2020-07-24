using System;
using System.IO;
using System.Text.Json.Serialization;

using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class File
    {
        public File(Filepath path, DateTime lastWriteTimeUtc, Optional<HAL> links)
        {
            Path = path;
            LastWriteTimeUtc = lastWriteTimeUtc;
            Links = links.ValueOr(null);
        }

        public Filepath Path { get; }

        public DateTime LastWriteTimeUtc { get; }

        [JsonPropertyName("_links")]
        public HAL Links { get; }

        public static File FromFileInfo(FileInfo fileInfo)
            => FromFileInfo(fileInfo,
                selfUri: Optional<AbsoluteUri>.Empty);

        public static File FromFileInfo(FileInfo fileInfo, Optional<AbsoluteUri> selfUri)
            => new File(
                path: new Filepath(fileInfo.Name),
                lastWriteTimeUtc: fileInfo.LastWriteTimeUtc,
                links: selfUri.OnValue(uri => new HAL(uri)));
    }
}
