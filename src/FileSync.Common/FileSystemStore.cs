using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            => Directory.EnumerateFiles(Filepath).Select(file => new FileInfo(file));

        public IEnumerable<DirectoryInfo> GetDirectories()
            => Directory.EnumerateDirectories(Filepath).Select(dir => new DirectoryInfo(dir));

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
