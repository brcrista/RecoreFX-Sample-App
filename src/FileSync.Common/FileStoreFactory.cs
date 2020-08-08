namespace FileSync.Common
{
    public sealed class FileStoreFactory : IFileStoreFactory
    {
        private readonly SystemFilepath root;

        public FileStoreFactory(SystemFilepath root) => this.root = root;

        public IFileStore Create(SystemFilepath relativePath)
            => new FileSystemStore(root.Combine(relativePath));
    }
}
