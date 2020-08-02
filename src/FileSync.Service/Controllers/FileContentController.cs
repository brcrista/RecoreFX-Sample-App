using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/content/{path}")]
    public sealed class FileContentV1Controller : ControllerBase
    {
        private readonly FileStoreFactory fileStoreFactory;

        public FileContentV1Controller(FileStoreFactory fileStoreFactory)
        {
            this.fileStoreFactory = fileStoreFactory;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileAsync([FromRoute] string path)
        {
            var dirName = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new Filepath(dirName));

            var name = Path.GetFileName(path);
            var stream = await fileStore.ReadFileAsync(new Filepath(name));

            return File(stream, MediaTypeNames.Application.Octet);
        }

        [HttpPut]
        public async Task<IActionResult> UploadFileAsync([FromRoute] string path)
        {
            if (!Request.Body.CanRead)
            {
                return BadRequest();
            }

            var dirName = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new Filepath(dirName));

            var name = Path.GetFileName(path);
            await fileStore.WriteFileAsync(new Filepath(name), Request.Body);

            return CreatedAtAction(nameof(DownloadFileAsync), routeValues: new { path }, value: path);
        }
    }
}
