using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileSync.Common
{
    public interface IFileStore
    {
        /// <summary>
        /// Lists the files in the file store.
        /// </summary>
        public IEnumerable<FileInfo> GetFiles();

        /// <summary>
        /// Lists the directories in the file store.
        /// </summary>
        public IEnumerable<DirectoryInfo> GetDirectories();

        /// <summary>
        /// Streams the contents of a file in the store.
        /// </summary>
        public Task<Stream> ReadFileAsync(Filepath path);

        /// <summary>
        /// Writes the contents of a file in the store.
        /// </summary>
        public Task WriteFileAsync(Filepath path, Stream content);
    }
}
