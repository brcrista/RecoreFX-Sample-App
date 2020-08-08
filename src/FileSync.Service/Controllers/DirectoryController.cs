using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/listing")]
    public sealed class DirectoryV1Controller : ControllerBase
    {
        private readonly FileStoreFactory fileStoreFactory;
        private readonly IFileHasher fileHasher;

        public DirectoryV1Controller(FileStoreFactory fileStoreFactory, IFileHasher fileHasher)
        {
            this.fileStoreFactory = fileStoreFactory;
            this.fileHasher = fileHasher;
        }

        [HttpGet]
        public IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing([FromQuery] string path = ".")
        {
            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToFilepath();
            var fileStore = fileStoreFactory.Create(systemPath);
            foreach (var directoryInfo in fileStore.GetDirectories())
            {
                var relativePath = path + "/" + directoryInfo.Name;
                var listingUri = new RelativeUri($"api/v1/listing?path={relativePath}");

                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    parentDirectory: systemPath,
                    listingUri: listingUri);
            }

            foreach (var fileInfo in fileStore.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    parentDirectory: systemPath,
                    fileHasher,
                    isServiceFile: true);
            }
        }
    }
}