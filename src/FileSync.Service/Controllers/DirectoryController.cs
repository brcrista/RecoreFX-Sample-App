using System.IO;
using Microsoft.AspNetCore.Mvc;

using FileSync.Common.Filesystem;

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
        public IActionResult GetListing([FromQuery] string? path)
        {
            // Note: making `path` non-nullable and using a default value will cause
            // ASP.NET to require the query parameter
            path ??= ".";

            // Assume that `path` uses forward slashes
            var forwardSlashPath = new ForwardSlashFilepath(path);
            var systemPath = forwardSlashPath.ToSystemFilepath();

            if (!Directory.Exists(systemPath.ToString()))
            {
                return NotFound();
            }

            return Ok(directoryListingService.GetListing(systemPath));
        }
    }
}