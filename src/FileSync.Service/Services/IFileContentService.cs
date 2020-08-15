using System.IO;
using System.Threading.Tasks;

using FileSync.Common;

namespace FileSync.Service
{
    public interface IFileContentService
    {
        Task<Stream> ReadFileContentsAsync(SystemFilepath systemPath);

        Task WriteFileContentsAsync(SystemFilepath systemPath, Stream contents);
    }
}