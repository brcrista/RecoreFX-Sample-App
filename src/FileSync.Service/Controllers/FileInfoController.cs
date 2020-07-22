using System.Collections.Generic;
using System.IO;

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
        [HttpGet]
        public IEnumerable<Models.File> GetFiles()
        {
            foreach (var file in Directory.EnumerateFiles("."))
            {
                var fileInfo = new FileInfo(file);
                var selfUri = new AbsoluteUri(HttpContext.Request.GetEncodedUrl()).Combine(fileInfo.Name);

                yield return new Models.File(
                    path: new Models.Path(fileInfo.Name),
                    lastWriteTimeUtc: fileInfo.LastWriteTimeUtc,
                    links: new Models.HAL(selfUri));
            }
        }
    }
}