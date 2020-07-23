using System.Collections.Generic;
using System.IO;

namespace FileSync.Common
{
    public interface IFileStore
    {
        public IEnumerable<FileInfo> GetFiles();
    }
}
