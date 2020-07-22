using System;

using Recore;

namespace FileSync.Service.Models
{
    public class Path : Of<string>
    {
        public Path(string value) => Value = value;
    }

    public sealed class File
    {
        public File(Path path, DateTime lastWriteTimeUtc, HAL links)
        {
            Path = path;
            LastWriteTimeUtc = lastWriteTimeUtc;
            _links = links;
        }

        public Path Path { get; }

        public DateTime LastWriteTimeUtc { get; }

        public HAL _links { get; } 
    }
}
