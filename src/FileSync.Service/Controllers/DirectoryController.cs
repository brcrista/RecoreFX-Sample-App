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
            var fileStore = fileStoreFactory.Create(new Filepath(path));
            foreach (var directoryInfo in fileStore.GetDirectories())
            {
                var relativePath = path + "/" + directoryInfo.Name;
                var listingUri = new RelativeUri($"api/v1/listing?path={relativePath}");

                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    relativePath: new Filepath(path),
                    listingUri: listingUri);
            }

            foreach (var fileInfo in fileStore.GetFiles())
            {
                var relativePath = path + "/" + fileInfo.Name;
                var contentUri = new RelativeUri($"api/v1/content?path={relativePath}");

                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    relativePath: new Filepath(path),
                    fileHasher,
                    contentUri: contentUri);
            }
        }
    }
}