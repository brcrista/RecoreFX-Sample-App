using System.IO;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common.ApiModels;

namespace FileSync.Service
{
    [ApiController]
    [Route("api/v1/listing")]
    public sealed class DirectoryV1Controller : ControllerBase
    {
        private readonly IDirectoryListingService directoryListingService;

        public DirectoryV1Controller(IDirectoryListingService directoryListingService)
        {
            this.directoryListingService = directoryListingService;
        }

        [HttpGet]
        public IActionResult GetListing([FromQuery] string path = ".")
        {
            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToFilepath();

            // Fail fast if the directory doesn't exist
            if (!Directory.Exists(systemPath))
            {
                return NotFound();
            }

            return Ok(directoryListingService.GetListing(systemPath));
        }
    }
}