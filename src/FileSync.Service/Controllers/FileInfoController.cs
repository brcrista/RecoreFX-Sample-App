using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using Recore;

using FileSync.Common.ApiModels;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/files/info")]
    public sealed class FileInfoV1Controller : ControllerBase
    {
        private readonly Services.IFileService fileService;

        public FileInfoV1Controller(Services.IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet]
        public IEnumerable<File> GetFiles()
        {
            foreach (var fileInfo in fileService.GetFiles())
            {
                var selfUri = new AbsoluteUri(HttpContext.Request.GetEncodedUrl()).Combine(fileInfo.Name);

                yield return new File(
                    path: new Path(fileInfo.Name),
                    lastWriteTimeUtc: fileInfo.LastWriteTimeUtc,
                    links: new HAL(selfUri));
            }
        }
    }
}