using System.IO;
using System.Threading.Tasks;

using FileSync.Common;

namespace FileSync.Service
{
    sealed class FileContentService : IFileContentService
    {
        private readonly IFileStoreFactory fileStoreFactory;

        public FileContentService(IFileStoreFactory fileStoreFactory)
        {
            this.fileStoreFactory = fileStoreFactory;
        }

        public async Task<Stream> ReadFileContentsAsync(string path)
        {
            var dirname = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

            var basename = Path.GetFileName(path);
            return await fileStore.ReadFileAsync(basename);
        }

        public async Task WriteFileContentsAsync(string path, Stream contents)
        {
            var dirname = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

            var basename = Path.GetFileName(path);
            await fileStore.WriteFileAsync(basename, contents);
        }
    }
}
