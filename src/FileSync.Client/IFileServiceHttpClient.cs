using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    interface IFileServiceHttpClient
    {
        Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync();

        Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync(RelativeUri listingUri);

        Task<Stream> GetFileContentAsync(FileSyncFile file);

        Task PutFileContentAsync(Filepath path, Stream content);
    }
}
