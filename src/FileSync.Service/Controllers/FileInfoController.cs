using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using Recore;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("files/info")]
    public sealed class FileInfoController : ControllerBase
    {
        private readonly Services.IFileService fileService;

        public FileInfoController(Services.IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet]
        public IEnumerable<Models.File> GetFiles()
        {
            foreach (var fileInfo in fileService.GetFiles())
            {
                var selfUri = new AbsoluteUri(HttpContext.Request.GetEncodedUrl()).Combine(fileInfo.Name);

                yield return new Models.File(
                    path: new Models.Path(fileInfo.Name),
                    lastWriteTimeUtc: fileInfo.LastWriteTimeUtc,
                    links: new Models.HAL(selfUri));
            }
        }
    }
}