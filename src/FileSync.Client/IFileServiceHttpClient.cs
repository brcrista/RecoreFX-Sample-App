using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public interface IFileServiceHttpClient
    {
        Task<IEnumerable<ApiModels.File>> GetFileInfosAsync();

        Task<Stream> GetFileContentAsync(ApiModels.File file);

        Task<ApiModels.File> PutFileContentAsync(Stream content);
    }
}
