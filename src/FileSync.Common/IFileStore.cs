using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common
{
    public interface IFileStore
    {
        public IEnumerable<FileInfo> GetFiles();

        public Task<Stream> ReadFileAsync(Filepath path);

        public Task WriteFileAsync(Filepath path, Stream content);
    }
}
