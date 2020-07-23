using System.Collections.Generic;
using System.IO;

namespace FileSync.Service.Services
{
    public interface IFileService
    {
        public IEnumerable<FileInfo> GetFiles();
    }
}
