using System;
using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory : IEquatable<FileSyncDirectory>
    {
        public ForwardSlashFilepath RelativePath { get; set; }

        public string ListingUrl { get; set; }

        public override int GetHashCode()
            => HashCode.Combine(RelativePath, ListingUrl);

        public override bool Equals(object obj)
            => obj is FileSyncDirectory fileSyncDirectory
            && Equals(fileSyncDirectory);

        public bool Equals(FileSyncDirectory other)
            => other != null
            && RelativePath == other.RelativePath
            && ListingUrl == other.ListingUrl;

        public static bool operator ==(FileSyncDirectory lhs, FileSyncDirectory rhs) => Equals(lhs, rhs);

        public static bool operator !=(FileSyncDirectory lhs, FileSyncDirectory rhs) => !Equals(lhs, rhs);

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, SystemFilepath parentDirectory, RelativeUri listingEndpoint)
        {
            var relativePath = ForwardSlashFilepath.FromSystemFilepath(parentDirectory).Combine(directoryInfo.Name);
            return new FileSyncDirectory
            {
                RelativePath = relativePath,
                ListingUrl = $"{listingEndpoint}?path={relativePath}"
            };
        }
    }
}
