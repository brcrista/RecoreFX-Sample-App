using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Recore;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/listing/{path}")]
    public sealed class DirectoryV1Controller : ControllerBase
    {
        private readonly FileStoreFactory fileStoreFactory;

        public DirectoryV1Controller(FileStoreFactory fileStoreFactory)
        {
            this.fileStoreFactory = fileStoreFactory;
        }

        [HttpGet]
        public IEnumerable<Either<FileSyncDirectory, FileSyncFile>> GetListing([FromRoute] string path)
        {
            var fileStore = fileStoreFactory.Create(new Filepath(path));
            foreach (var directoryInfo in fileStore.GetDirectories())
            {
                var requestUrl = HttpContext.Request.GetEncodedUrl();
                yield return FileSyncDirectory.FromDirectoryInfo(
                    directoryInfo,
                    relativePath: new Filepath(path),
                    listingUri: new AbsoluteUri(requestUrl).Combine(directoryInfo.Name));
            }

            foreach (var fileInfo in fileStore.GetFiles())
            {
                var requestUrl = HttpContext.Request.GetEncodedUrl();
                yield return FileSyncFile.FromFileInfo(
                    fileInfo,
                    relativePath: new Filepath(path),
                    contentUri: new AbsoluteUri(requestUrl).Combine(fileInfo.Name));
            }
        }
    }
}