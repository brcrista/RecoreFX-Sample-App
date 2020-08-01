using System;
using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncFile
    {
        public Filepath RelativePath { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        public Optional<string> ContentUrl { get; set; }

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath)
            => FromFileInfo(fileInfo, relativePath, contentUri: Optional<RelativeUri>.Empty);

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath, Optional<RelativeUri> contentUri)
            => new FileSyncFile
            {
                RelativePath = relativePath.Combine(new Filepath(fileInfo.Name)),
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                ContentUrl = contentUri.OnValue(x => x.ToString())
            };
    }
}
