using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            var stream = await fileContentService.ReadFileContentsAsync(path);
            return File(stream, MediaTypeNames.Application.Octet);
        }

        [HttpPut]
        public async Task<IActionResult> UploadFileAsync([FromQuery] string path)
        {
            if (!Request.Body.CanRead)
            {
                return BadRequest();
            }

            await fileContentService.WriteFileContentsAsync(path, Request.Body);
            return CreatedAtAction(nameof(DownloadFileAsync), routeValues: new { path }, value: path);
        }
    }
}
