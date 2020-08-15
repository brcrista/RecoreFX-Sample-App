using System.Collections.Generic;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Service
{
    public interface IDirectoryListingService
    {
        IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing(SystemFilepath systemPath);
    }
}