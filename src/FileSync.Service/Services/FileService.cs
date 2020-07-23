using System.Collections.Generic;
using System.IO;

namespace FileSync.Service.Services
{
    public sealed class FileService : IFileService
    {
        public IEnumerable<FileInfo> GetFiles()
        {
            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                yield return new FileInfo(file);
            }
        }
    }
}
