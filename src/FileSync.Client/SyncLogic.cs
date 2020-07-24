using System.Collections.Generic;
using System.Linq;

using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class SyncLogic
    {
        private IReadOnlyList<File> clientFiles;
        private IReadOnlyList<File> serverFiles;

        public SyncLogic(IEnumerable<File> clientFiles, IEnumerable<File> serverFiles)
        {
            this.clientFiles = clientFiles.ToList();
            this.serverFiles = serverFiles.ToList();
        }

        public IEnumerable<File> FilesToDownload()
        {
            // TODO
            return Enumerable.Empty<File>();
        }

        public IEnumerable<File> FilesToUpload()
        {
            // TODO
            return Enumerable.Empty<File>();
        }

        public IEnumerable<Conflict> Conflicts()
        {
            // TODO
            // Choose newer timestamp
            return Enumerable.Empty<Conflict>();
        }
    }
}
