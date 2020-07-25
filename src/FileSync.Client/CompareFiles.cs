using System.Collections.Generic;
using System.Linq;

using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class CompareFiles
    {
        private readonly IReadOnlyList<File> clientFiles;
        private readonly IReadOnlyList<File> serverFiles;

        public CompareFiles(IEnumerable<File> clientFiles, IEnumerable<File> serverFiles)
        {
            this.clientFiles = clientFiles.ToList();
            this.serverFiles = serverFiles.ToList();
        }

        public IEnumerable<File> FilesToDownload()
        {
            // TODO
            return serverFiles;
        }

        public IEnumerable<File> FilesToUpload()
        {
            // TODO
            return clientFiles;
        }

        public IEnumerable<Conflict> Conflicts()
        {
            // TODO
            // Choose newer timestamp
            return Enumerable.Empty<Conflict>();
        }
    }
}
