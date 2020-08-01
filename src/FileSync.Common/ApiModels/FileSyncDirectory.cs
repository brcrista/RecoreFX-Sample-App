using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncDirectory
    {
        public Filepath RelativePath { get; set; }
        public AbsoluteUri Listing { get; set; }

        public static FileSyncDirectory FromDirectoryInfo(DirectoryInfo directoryInfo, Filepath relativePath, AbsoluteUri listingUri)
            => new FileSyncDirectory
            {
                RelativePath = new Filepath(Path.Combine(relativePath, directoryInfo.Name)),
                Listing = listingUri
            };
    }
}
