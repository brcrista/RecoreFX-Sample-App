using System.Collections.Generic;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Service
{
    sealed class DirectoryListingService : IDirectoryListingService
    {
        private readonly IFileStoreFactory fileStoreFactory;
        private readonly IFileHasher fileHasher;

        public DirectoryListingService(IFileStoreFactory fileStoreFactory, IFileHasher fileHasher)
        {
            this.fileStoreFactory = fileStoreFactory;
            this.fileHasher = fileHasher;
        }

        public IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing(SystemFilepath systemPath)
        {
            var fileStore = fileStoreFactory.Create(systemPath);
            foreach (var directoryInfo in fileStore.GetDirectories())
            {
                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    parentDirectory: systemPath,
                    listingEndpoint: new RelativeUri($"api/v1/listing"));
            }

            foreach (var fileInfo in fileStore.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    parentDirectory: systemPath,
                    Optional.Of(fileHasher),
                    contentEndpoint: new RelativeUri("api/v1/content"));
            }
        }
    }
}
