using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/files/{path}/content")]
    public sealed class FileContentV1Controller : ControllerBase
    {
        private readonly IFileStore fileStore;

        public FileContentV1Controller(IFileStore fileStore)
        {
            this.fileStore = fileStore;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileAsync([FromRoute] string path)
        {
            var stream = await fileStore.ReadFileAsync(new Filepath(path));
            return File(stream, MediaTypeNames.Application.Octet);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync([FromRoute] string path, [FromBody] byte[] contents)
        {
            await fileStore.WriteFileAsync(new Filepath(path), new MemoryStream(contents));
            return CreatedAtAction(nameof(DownloadFileAsync), routeValues: new { path }, value: path);
        }
    }
}
