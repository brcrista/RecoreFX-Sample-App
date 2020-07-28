using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common
{
    /// <summary>
    /// A file store backed by the local file system.
    /// The file store consists of the whole subtree rooted at a given filepath.
    /// </summary>
    public sealed class FileSystemStore : IFileStore
    {
        public Filepath Root { get; }

        public FileSystemStore(Filepath root)
        {
            Root = root;
        }

        public IEnumerable<ApiModels.File> GetFiles()
        {
            var files = Directory.EnumerateFiles(
                path: Root.Value,
                searchPattern: "*");
                //enumerationOptions: new EnumerationOptions { RecurseSubdirectories = true });

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                yield return new ApiModels.File
                {
                    Path = new Filepath(fileInfo.Name),
                    LastWriteTimeUtc = fileInfo.LastWriteTimeUtc
                };
            }
        }

        public Task<Stream> ReadFileAsync(Filepath relativePath)
        {
            var pathInStore = Path.Join(Root.Value, relativePath.Value);
            return Task.FromResult<Stream>(File.OpenRead(pathInStore));
        }

        public async Task WriteFileAsync(Filepath relativePath, Stream content)
        {
            var pathInStore = Path.Join(Root.Value, relativePath.Value);
            using var filestream = File.OpenWrite(pathInStore);
            await content.CopyToAsync(filestream);
        }
    }
}
