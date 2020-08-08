using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory
    {
        public ForwardSlashFilepath RelativePath { get; set; }
        public string ListingUrl { get; set; }

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
