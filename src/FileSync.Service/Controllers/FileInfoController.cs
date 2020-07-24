using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Recore;

using FileSync.Common;
using ApiModels = FileSync.Common.ApiModels;

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
        public IEnumerable<ApiModels.File> GetFiles()
        {
            foreach (var fileInfo in fileStore.GetFiles())
            {
                var requestUrl = HttpContext.Request.GetEncodedUrl();
                yield return ApiModels.File.FromFileInfo(
                    fileInfo,
                    selfUri: new AbsoluteUri(requestUrl).Combine(fileInfo.Name));
            }
        }
    }
}