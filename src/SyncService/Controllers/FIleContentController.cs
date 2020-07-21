using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SyncService.Controllers
{
    [ApiController]
    [Route("files/content")]
    public sealed class FIleContentController : ControllerBase
    {
        [HttpGet]
        public Task<FileStreamResult> DownloadFileAsync([FromRoute] string path)
        {
            // TODO
            using var stream = new MemoryStream();
            return Task.FromResult(File(stream, MediaTypeNames.Application.Octet));
        }

        [HttpPost]
        public Task<CreatedAtActionResult> UploadFileAsync([FromRoute] string path, [FromBody] byte[] contents)
        {
            // TODO
            return Task.FromResult(CreatedAtAction(nameof(DownloadFileAsync), new { path }, path));
        }
    }
}
