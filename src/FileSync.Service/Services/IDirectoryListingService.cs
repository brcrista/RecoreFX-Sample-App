using System.Collections.Generic;
using Recore;

using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;

namespace FileSync.Service
{
    public interface IDirectoryListingService
    {
        IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing(SystemFilepath systemPath);
    }
}