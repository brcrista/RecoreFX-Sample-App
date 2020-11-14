namespace FileSync.Common
{
    public sealed class DirectoryFactory : IDirectoryFactory
    {
        private readonly SystemFilepath root;

        public DirectoryFactory(SystemFilepath root) => this.root = root;

        public IDirectory Create(SystemFilepath relativePath)
            => new FileSystemDirectory(root.Combine(relativePath));
    }
}
