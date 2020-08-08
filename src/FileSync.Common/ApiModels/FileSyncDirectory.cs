using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory
    {
        public ForwardSlashFilepath RelativePath { get; set; }
        public string ListingUrl { get; set; }

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, Filepath parentDirectory, RelativeUri listingUri)
            => new FileSyncDirectory
            {
                RelativePath = ForwardSlashFilepath.FromFilepath(parentDirectory).Combine(directoryInfo.Name),
                ListingUrl = listingUri.ToString()
            };
    }
}
