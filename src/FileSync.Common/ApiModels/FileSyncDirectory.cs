using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed record FileSyncDirectory
    {
        public FileSyncDirectory(ForwardSlashFilepath relativePath, string listingUrl)
        {
            RelativePath = relativePath;
            ListingUrl = listingUrl;
        }

        public ForwardSlashFilepath RelativePath { get; }

        public string ListingUrl { get; }

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, SystemFilepath parentDirectory, RelativeUri listingEndpoint)
        {
            var relativePath = ForwardSlashFilepath.FromSystemFilepath(parentDirectory).Combine(directoryInfo.Name);
            return new FileSyncDirectory(relativePath, $"{listingEndpoint}?path={relativePath}");
        }
    }
}
