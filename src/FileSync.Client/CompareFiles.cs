using System.Collections.Generic;
using System.Linq;

using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class CompareFiles
    {
        private readonly Dictionary<ForwardSlashFilepath, FileSyncFile> clientFiles;
        private readonly Dictionary<ForwardSlashFilepath, FileSyncFile> serviceFiles;

        public CompareFiles(IEnumerable<FileSyncFile> clientFiles, IEnumerable<FileSyncFile> serviceFiles)
        {
            this.clientFiles = clientFiles.ToDictionary(x => x.RelativePath);
            this.serviceFiles = serviceFiles.ToDictionary(x => x.RelativePath);
        }

        public IEnumerable<FileSyncFile> FilesToDownload()
            => OnlyOnService().Concat(
                Conflicts()
                    .Where(conflict => conflict.ChosenVersion == ChosenVersion.Service)
                    .Select(conflict => conflict.ServiceFile));

        public IEnumerable<FileSyncFile> FilesToUpload()
            => OnlyOnClient().Concat(
                Conflicts()
                    .Where(conflict => conflict.ChosenVersion == ChosenVersion.Client)
                    .Select(conflict => conflict.ClientFile));

        public IEnumerable<Conflict> Conflicts()
            => clientFiles.Keys
                .Intersect(serviceFiles.Keys)
                .Select(key => new Conflict(
                    clientFile: clientFiles[key],
                    serviceFile: serviceFiles[key]));

        private IEnumerable<FileSyncFile> OnlyOnService()
            => serviceFiles.Keys
                .Except(clientFiles.Keys)
                .Select(key => serviceFiles[key]);

        private IEnumerable<FileSyncFile> OnlyOnClient()
            => clientFiles.Keys
                .Except(serviceFiles.Keys)
                .Select(key => clientFiles[key]);
    }
}
