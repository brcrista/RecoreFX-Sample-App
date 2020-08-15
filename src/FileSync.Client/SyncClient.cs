using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Recore;
using Recore.Collections.Generic;

using FileSync.Client.UI;
using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    /// <summary>
    /// Implements the high-level logic for the file sync client.
    /// </summary>
    sealed class SyncClient
    {
        private readonly ITextView view;
        private readonly IFileStoreFactory fileStoreFactory;
        private readonly IFileHasher fileHasher;
        private readonly IFileServiceApi fileService;

        public SyncClient(
            ITextView view,
            IFileStoreFactory fileStoreFactory,
            IFileHasher fileHasher,
            IFileServiceApi fileService)
        {
            this.view = view;
            this.fileStoreFactory = fileStoreFactory;
            this.fileHasher = fileHasher;
            this.fileService = fileService;
        }

        public async Task RunAsync()
        {
            // Get the files on the client
            var filesOnClient = GetAllFilesOnClient(new SystemFilepath(".")).ToList();
            view.Verbose(new FileListViewComponent("Files on the client:", filesOnClient));

            // Call the service to get the files on it
            var filesOnService = (await GetAllFilesOnService()).ToList();
            view.Verbose(new FileListViewComponent("Files on the service:", filesOnService));

            // Find and resolve conflicts
            var compareFiles = new CompareFiles(filesOnClient, filesOnService, fileHasher);
            view.Out(new ConflictsViewComponent(compareFiles.Conflicts()));

            // Upload files to the service
            var filesToUpload = compareFiles.FilesToUpload().ToList();
            view.Verbose(new FileListViewComponent("Files to upload:", filesToUpload));

            foreach (var file in filesToUpload)
            {
                var dirname = Path.GetDirectoryName(file.RelativePath);
                var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

                var basename = Path.GetFileName(file.RelativePath);
                var content = await fileStore.ReadFileAsync(basename);

                await fileService.PutFileContentAsync(file.RelativePath, content);
            }

            // Download file content from the service
            var filesToDownload = compareFiles.FilesToDownload().ToList();
            view.Verbose(new FileListViewComponent("Files to download:", filesToDownload));

            foreach (var file in filesToDownload)
            {
                var dirname = Path.GetDirectoryName(file.RelativePath);
                var fileStore = fileStoreFactory.Create(new SystemFilepath(dirname));

                var basename = Path.GetFileName(file.RelativePath);
                var content = await fileService.GetFileContentAsync(file);

                await fileStore.WriteFileAsync(basename, content);
            }

            // Print summary
            var compareOnFilepath = new MappedEqualityComparer<FileSyncFile, ForwardSlashFilepath>(x => x.RelativePath);
            view.Out(new SummaryViewComponent(
                sentFiles: filesToUpload,
                newFiles: filesToDownload.Except(filesOnClient, compareOnFilepath).ToList(),
                changedFiles: filesToDownload.Intersect(filesOnClient, compareOnFilepath).ToList()));
        }

        private IEnumerable<FileSyncFile> GetAllFilesOnClient(SystemFilepath currentDirectory)
        {
            var fileStore = fileStoreFactory.Create(currentDirectory);
            foreach (var file in fileStore.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(file, currentDirectory);
            }

            var directories = fileStore.GetDirectories();
            foreach (var directory in directories)
            {
                var subdirectory = currentDirectory.Combine(directory.Name);
                var filesInSubdir = GetAllFilesOnClient(subdirectory);
                foreach (var file in filesInSubdir)
                {
                    yield return file;
                }
            }
        }

        private async Task<IEnumerable<FileSyncFile>> GetAllFilesOnService()
        {
            async Task<IEnumerable<FileSyncFile>> GetServiceFilesRecursive(Optional<RelativeUri> listingUri)
            {
                var listing = await fileService.GetDirectoryListingAsync(listingUri);

                var result = new LinkedList<FileSyncFile>();
                foreach (var entry in listing)
                {
                    var files = await entry.Switch(
                        async dir => await GetServiceFilesRecursive(new RelativeUri(dir.ListingUrl)),
                        file => Task.FromResult<IEnumerable<FileSyncFile>>(new[] { file }));

                    foreach (var file in files)
                    {
                        result.AddLast(file);
                    }
                }

                return result;
            }

            return await GetServiceFilesRecursive(Optional<RelativeUri>.Empty);
        }
    }
}
