using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("files/content")]
    public sealed class FileContentController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> DownloadFileAsync([FromRoute] string path)
        {
            // TODO
            using var stream = new MemoryStream();
            return Task.FromResult<IActionResult>(File(stream, MediaTypeNames.Application.Octet));
        }

        [HttpPost]
        public Task<IActionResult> UploadFileAsync([FromRoute] string path, [FromBody] byte[] contents)
        {
            // TODO
            return Task.FromResult<IActionResult>(CreatedAtAction(nameof(DownloadFileAsync), new { path }, path));
        }
    }
}
