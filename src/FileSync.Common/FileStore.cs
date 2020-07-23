using System.Collections.Generic;
using System.IO;

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
    }
}
