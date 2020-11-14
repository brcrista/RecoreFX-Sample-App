using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common.ApiModels;

namespace FileSync.Service
{
    [ApiController]
    [Route("api/v1/content")]
    public sealed class FileContentV1Controller : ControllerBase
    {
        private readonly IFileContentService fileContentService;

        public FileContentV1Controller(IFileContentService fileContentService)
        {
            this.fileContentService = fileContentService;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileAsync([FromQuery] string path)
        {
            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToFilepath();

            if (!System.IO.File.Exists(systemPath.ToString()))
            {
                return NotFound();
            }

            var stream = await fileContentService.ReadFileContentsAsync(systemPath);
            return File(stream, MediaTypeNames.Application.Octet);
        }

        [HttpPut]
        public async Task<IActionResult> UploadFileAsync([FromQuery] string path)
        {
            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToFilepath();

            if (!Request.Body.CanRead)
            {
                return BadRequest();
            }

            await fileContentService.WriteFileContentsAsync(systemPath, Request.Body);
            return CreatedAtAction(nameof(DownloadFileAsync), routeValues: new { path }, value: path);
        }
    }
}
