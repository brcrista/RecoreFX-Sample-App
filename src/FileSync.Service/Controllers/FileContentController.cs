using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common;

namespace FileSync.Service.Controllers
{
    [ApiController]
    [Route("api/v1/content")]
    public sealed class FileContentV1Controller : ControllerBase
    {
        private readonly FileStoreFactory fileStoreFactory;

        public FileContentV1Controller(FileStoreFactory fileStoreFactory)
        {
            this.fileStoreFactory = fileStoreFactory;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileAsync([FromQuery] string path)
        {
            var dirname = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new Filepath(dirname));

            var basename = Path.GetFileName(path);
            var stream = await fileStore.ReadFileAsync(basename);

            return File(stream, MediaTypeNames.Application.Octet);
        }

        [HttpPut]
        public async Task<IActionResult> UploadFileAsync([FromQuery] string path)
        {
            if (!Request.Body.CanRead)
            {
                return BadRequest();
            }

            var dirname = Path.GetDirectoryName(path);
            var fileStore = fileStoreFactory.Create(new Filepath(dirname));

            var basename = Path.GetFileName(path);
            await fileStore.WriteFileAsync(basename, Request.Body);

            return CreatedAtAction(nameof(DownloadFileAsync), routeValues: new { path }, value: path);
        }
    }
}
