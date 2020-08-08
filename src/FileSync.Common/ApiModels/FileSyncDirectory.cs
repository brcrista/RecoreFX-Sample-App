using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory
    {
        public ForwardSlashFilepath RelativePath { get; set; }
        public string ListingUrl { get; set; }

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, Filepath parentDirectory, RelativeUri listingEndpoint)
        {
            var relativePath = ForwardSlashFilepath.FromFilepath(parentDirectory).Combine(directoryInfo.Name);
            return new FileSyncDirectory
            {
                RelativePath = relativePath,
                ListingUrl = $"{listingEndpoint}?path={relativePath}"
            };
        }
    }
}
