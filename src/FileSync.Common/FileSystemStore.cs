using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common
{
    /// <summary>
    /// A file store backed by the local file system.
    /// </summary>
    public sealed class FileSystemStore : IFileStore
    {
        public Filepath Filepath { get; }

        public FileSystemStore(Filepath filepath)
        {
            Filepath = filepath;
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            foreach (var file in Directory.EnumerateFiles(Filepath.Value))
            {
                yield return new FileInfo(file);
            }
        }

        public Task<Stream> ReadFileAsync(Filepath relativePath)
        {
            var pathInStore = Path.Join(Filepath.Value, relativePath.Value);
            return Task.FromResult<Stream>(File.OpenRead(pathInStore));
        }

        public async Task WriteFileAsync(Filepath relativePath, Stream content)
        {
            var pathInStore = Path.Join(Filepath.Value, relativePath.Value);
            using var filestream = File.OpenWrite(pathInStore);
            await content.CopyToAsync(filestream);
        }
    }
}
