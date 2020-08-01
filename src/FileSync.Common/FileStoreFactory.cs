using System.IO;

namespace FileSync.Common
{
    // This class exists for ASP.NET dependency injection,
    // which won't take a factory function.
    public sealed class FileStoreFactory
    {
        private readonly Filepath root;

        public FileStoreFactory(Filepath root) => this.root = root;

        public IFileStore Create(Filepath relativePath)
        {
            var fileStorePath = Path.Combine(root, relativePath);
            return new FileSystemStore(new Filepath(fileStorePath));
        }
    }
}
