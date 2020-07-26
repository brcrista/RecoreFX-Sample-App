using System.Collections.Generic;
using System.Linq;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class CompareFiles
    {
        private readonly Dictionary<Filepath, File> clientFiles;
        private readonly Dictionary<Filepath, File> serviceFiles;

        public CompareFiles(IEnumerable<File> clientFiles, IEnumerable<File> serviceFiles)
        {
            this.clientFiles = clientFiles.ToDictionary(x => x.Path);
            this.serviceFiles = serviceFiles.ToDictionary(x => x.Path);
        }

        public IEnumerable<File> FilesToDownload()
            => serviceFiles.Keys
                .Except(clientFiles.Keys)
                .Select(key => serviceFiles[key]);

        public IEnumerable<File> FilesToUpload()
            => clientFiles.Keys
                .Except(serviceFiles.Keys)
                .Select(key => clientFiles[key]);

        public IEnumerable<Conflict> Conflicts()
            => clientFiles.Keys
                .Intersect(serviceFiles.Keys)
                .Select(key => new Conflict(
                    clientFile: clientFiles[key],
                    serviceFile: serviceFiles[key]));
    }
}
