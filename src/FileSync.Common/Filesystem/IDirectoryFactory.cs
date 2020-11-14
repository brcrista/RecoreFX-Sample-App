namespace FileSync.Common.Filesystem
{
    // This interface exists for ASP.NET dependency injection,
    // which won't take a simple function as a factory.
    public interface IDirectoryFactory
    {
        IDirectory Open(SystemFilepath relativePath);
    }
}