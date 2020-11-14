using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common.Filesystem
{
    public interface IDirectory
    {
        /// <summary>
        /// Lists the files in the file store.
        /// </summary>
        IEnumerable<FileInfo> GetFiles();

        /// <summary>
        /// Lists the directories in the file store.
        /// </summary>
        IEnumerable<DirectoryInfo> GetSubdirectories();

        /// <summary>
        /// Streams the contents of a file in the store.
        /// </summary>
        Task<Stream> ReadFileAsync(string filename);

        /// <summary>
        /// Writes the contents of a file in the store.
        /// </summary>
        Task WriteFileAsync(string filename, Stream content);
    }
}
