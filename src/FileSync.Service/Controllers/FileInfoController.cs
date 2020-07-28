using System;
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
    [Route("api/v1/files")]
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
            foreach (var file in fileStore.GetFiles())
            {
                // Set the HAL URLs for the file
                var requestUrl = HttpContext.Request.GetEncodedUrl();
                var selfUrl = new AbsoluteUri(requestUrl).Combine(file.Path.Value);
                file.Links = ApiModels.HAL.Create(
                    selfUrl,
                    new Dictionary<string, Uri> { ["content"] = selfUrl.Combine("content") });

                yield return file;
            }
        }
    }
}