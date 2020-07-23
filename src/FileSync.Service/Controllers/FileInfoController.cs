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
    [Route("api/v1/files/info")]
    public sealed class FileInfoV1Controller : ControllerBase
    {
        private readonly IFileStore fileStore;

        public FileInfoV1Controller(IFileStore fileStore)
        {
            this.fileStore = fileStore;
        }

        [HttpGet]
        public IEnumerable<File> GetFiles()
        {
            foreach (var fileInfo in fileStore.GetFiles())
            {
                var selfUri = new AbsoluteUri(HttpContext.Request.GetEncodedUrl()).Combine(fileInfo.Name);

                yield return new File(
                    path: new Filepath(fileInfo.Name),
                    lastWriteTimeUtc: fileInfo.LastWriteTimeUtc,
                    links: new HAL(selfUri));
            }
        }
    }
}