using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class FileServiceHttpClient
    {
        public Task<IEnumerable<ApiModels.File>> GetFileInfoAsync()
        {
            // TODO
            return Task.FromResult(System.Linq.Enumerable.Empty<ApiModels.File>());
        }

        public Task<Stream> GetFileContentAsync(ApiModels.File file)
        {
            // TODO
            var uri = file.Links["self"].Href;
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<ApiModels.File> PutFileContentAsync(Stream content)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
