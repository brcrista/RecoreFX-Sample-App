using System.IO;
using System.Threading.Tasks;

using FileSync.Common;

namespace FileSync.Service
{
    sealed class FileContentService : IFileContentService
    {
        private readonly IDirectoryFactory directoryFactory;

        public FileContentService(IDirectoryFactory directoryFactory)
        {
            this.directoryFactory = directoryFactory;
        }

        public async Task<Stream> ReadFileContentsAsync(SystemFilepath systemPath)
        {
            var (dirname, basename) = systemPath.Split();
            var directory = directoryFactory.Open(dirname);
            return await directory.ReadFileAsync(basename);
        }

        public async Task WriteFileContentsAsync(SystemFilepath systemPath, Stream contents)
        {
            var (dirname, basename) = systemPath.Split();
            var directory = directoryFactory.Open(dirname);
            await directory.WriteFileAsync(basename, contents);
        }
    }
}
