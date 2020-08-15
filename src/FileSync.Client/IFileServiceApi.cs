using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Recore;

using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    interface IFileServiceApi
    {
        Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync(Optional<RelativeUri> listingUri);

        Task<Stream> GetFileContentAsync(FileSyncFile file);

        Task PutFileContentAsync(ForwardSlashFilepath path, Stream content);
    }
}
