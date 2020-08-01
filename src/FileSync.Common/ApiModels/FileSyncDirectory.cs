using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory
    {
        public Filepath RelativePath { get; set; }
        public string ListingUrl { get; set; }

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, Filepath relativePath, AbsoluteUri listingUri)
            => new FileSyncDirectory
            {
                RelativePath = relativePath.Combine(new Filepath(directoryInfo.Name)),
                ListingUrl = listingUri.AbsoluteUri
            };
    }
}
