using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common
{
    public sealed class FileStore : IFileStore
    {
        public Filepath Path { get; }

        public FileStore(Filepath path)
        {
            Path = path;
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            foreach (var file in Directory.EnumerateFiles(Path.Value))
            {
                yield return new FileInfo(file);
            }
        }

        public Task<Stream> ReadFileAsync(Filepath path)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteFileAsync(Filepath path, Stream content)
        {
            throw new System.NotImplementedException();
        }
    }
}
