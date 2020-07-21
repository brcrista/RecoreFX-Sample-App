using System;

namespace FileSync.Service.Models
{
    public sealed class File
    {
        public File(string path, DateTime lastWriteTimeUtc, HAL links)
        {
            Path = path;
            LastWriteTimeUtc = lastWriteTimeUtc;
            _links = links;
        }

        public string Path { get; }

        public DateTime LastWriteTimeUtc { get; }

        public HAL _links { get; } 
    }
}
