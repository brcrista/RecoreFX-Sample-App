namespace FileSync.Common
{
    // This class exists for ASP.NET dependency injection,
    // which won't take a factory function.
    public sealed class FileStoreFactory
    {
        private readonly SystemFilepath root;

        public FileStoreFactory(SystemFilepath root) => this.root = root;

        public IFileStore Create(SystemFilepath relativePath)
            => new FileSystemStore(root.Combine(relativePath));
    }
}
