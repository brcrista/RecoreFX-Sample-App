using System.Collections.Generic;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;

namespace FileSync.Service
{
    sealed class DirectoryListingService : IDirectoryListingService
    {
        private readonly IDirectoryFactory directoryFactory;
        private readonly IFileHasher fileHasher;

        public DirectoryListingService(IDirectoryFactory directoryFactory, IFileHasher fileHasher)
        {
            this.directoryFactory = directoryFactory;
            this.fileHasher = fileHasher;
        }

        public IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing(SystemFilepath systemPath)
        {
            var directory = directoryFactory.Open(systemPath);
            foreach (var directoryInfo in directory.GetSubdirectories())
            {
                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    parentDirectory: systemPath,
                    listingEndpoint: Endpoints.Listing);
            }

            foreach (var fileInfo in directory.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    parentDirectory: systemPath,
                    fileHasher,
                    contentEndpoint: Endpoints.Content);
            }
        }
    }
}
