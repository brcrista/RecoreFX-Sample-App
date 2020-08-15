using System.IO;
using System.Threading.Tasks;

namespace FileSync.Service
{
    public interface IFileContentService
    {
        Task<Stream> ReadFileContentsAsync(string path);
        Task WriteFileContentsAsync(string path, Stream contents);
    }
}