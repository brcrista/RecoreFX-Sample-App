using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSync.Common.Filesystem
{
    /// <summary>
    /// A directory on the local file system.
    /// </summary>
    sealed class FileSystemDirectory : IDirectory
    {
        public SystemFilepath Filepath { get; }

        public FileSystemDirectory(SystemFilepath filepath)
        {
            Directory.CreateDirectory(filepath.ToString());
            Filepath = filepath;
        }

        public IEnumerable<FileInfo> GetFiles()
            => Directory.EnumerateFiles(Filepath.ToString()).Select(file => new FileInfo(file));

        public IEnumerable<DirectoryInfo> GetSubdirectories()
            => Directory.EnumerateDirectories(Filepath.ToString()).Select(dir => new DirectoryInfo(dir));

        public Task<Stream> ReadFileAsync(string filename)
        {
            var pathInStore = Path.Join(Filepath.ToString(), filename);
            return Task.FromResult<Stream>(File.OpenRead(pathInStore));
        }

        public async Task WriteFileAsync(string filename, Stream content)
        {
            var pathInStore = Path.Join(Filepath.ToString(), filename);
            using var filestream = File.OpenWrite(pathInStore);
            await content.CopyToAsync(filestream);
        }
    }
}
