namespace FileSync.Common.Filesystem
{
    public sealed class DirectoryFactory : IDirectoryFactory
    {
        private readonly SystemFilepath root;

        public DirectoryFactory(SystemFilepath root) => this.root = root;

        public IDirectory Open(SystemFilepath relativePath)
            => new FileSystemDirectory(root.Combine(relativePath));
    }
}
