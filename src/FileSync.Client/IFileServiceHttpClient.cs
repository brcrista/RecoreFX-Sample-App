using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    interface IFileServiceHttpClient
    {
        Task<IEnumerable<FileSyncFile>> GetDirectoryListingAsync(Filepath path);

        Task<Stream> GetFileContentAsync(FileSyncFile file);

        Task PutFileContentAsync(Filepath path, Stream content);
    }
}
