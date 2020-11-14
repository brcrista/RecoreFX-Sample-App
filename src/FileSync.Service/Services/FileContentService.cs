using System;
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

        public async Task<Stream> ReadFileContentsAsync(SystemFilepath systemPath)
        {
            var dirname = Path.GetDirectoryName(systemPath.ToString());
            if (dirname is null)
            {
                throw new ArgumentException(
                    paramName: nameof(systemPath),
                    message: $"'{systemPath}' is not a directory with a parent.");
            }

            var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

            var basename = Path.GetFileName(systemPath.ToString());
            return await fileStore.ReadFileAsync(basename);
        }

        public async Task WriteFileContentsAsync(SystemFilepath systemPath, Stream contents)
        {
            var dirname = Path.GetDirectoryName(systemPath.ToString());
            if (dirname is null)
            {
                throw new ArgumentException(
                    paramName: nameof(systemPath),
                    message: $"'{systemPath}' is not a directory with a parent.");
            }

            var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

            var basename = Path.GetFileName(systemPath.ToString());
            await fileStore.WriteFileAsync(basename, contents);
        }
    }
}
