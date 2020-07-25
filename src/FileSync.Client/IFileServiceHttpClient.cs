using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    interface IFileServiceHttpClient
    {
        Task<IEnumerable<ApiModels.File>> GetAllFileInfoAsync();

        Task<Stream> GetFileContentAsync(ApiModels.File file);

        Task<ApiModels.File> PutFileContentAsync(Stream content);
    }
}
