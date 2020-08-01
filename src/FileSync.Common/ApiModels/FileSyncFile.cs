using System;
using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncFile
    {
        public Filepath RelativePath { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        public Optional<AbsoluteUri> Content { get; set; }

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath)
            => FromFileInfo(fileInfo, relativePath, contentUri: Optional<AbsoluteUri>.Empty);

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath, Optional<AbsoluteUri> contentUri)
            => new FileSyncFile
            {
                RelativePath = relativePath.Combine(new Filepath(fileInfo.Name)),
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                Content = contentUri
            };
    }
}
