namespace FileSync.Common
{
    // This class exists for ASP.NET dependency injection,
    // which won't take a factory function.
    public sealed class FileStoreFactory
    {
        private readonly Filepath root;

        public FileStoreFactory(Filepath root) => this.root = root;

        public IFileStore Create(Filepath relativePath)
            => new FileSystemStore(root.Combine(relativePath));
    }
}
