using System.Linq;
using System.Threading.Tasks;
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
        private readonly IFileStore fileStore;
        private readonly IFileServiceHttpClient fileService;

        public SyncClient(
            ITextView view,
            IFileStore fileStore,
            IFileServiceHttpClient fileService)
        {
            this.view = view;
            this.fileStore = fileStore;
            this.fileService = fileService;
        }

        public async Task RunAsync()
        {
            // Check the files in our directory
            var filesOnClient = fileStore.GetFiles().ToList();
            view.Verbose(new FileListViewComponent("Files on the client:", filesOnClient));

            // Call the service to get the files on it
            var filesOnService = (await fileService.GetAllFileInfoAsync()).ToList();
            view.Verbose(new FileListViewComponent("Files on the service:", filesOnService));

            var compareFiles = new CompareFiles(filesOnClient, filesOnService);

            // Print a message for conflicts
            view.Out(new ConflictsViewComponent(compareFiles.Conflicts()));

            // Download file content from the service
            var filesToDownload = compareFiles.FilesToDownload().ToList();
            view.Verbose(new FileListViewComponent("Files to download:", filesToDownload));

            foreach (var file in filesToDownload)
            {
                var content = await fileService.GetFileContentAsync(file);
                await fileStore.WriteFileAsync(file.Path, content);
            }

            // Upload files to the service
            var filesToUpload = compareFiles.FilesToUpload().ToList();
            view.Verbose(new FileListViewComponent("Files to upload:", filesToUpload));

            foreach (var file in filesToUpload)
            {
                var content = await fileStore.ReadFileAsync(file.Path);
                await fileService.PutFileContentAsync(file.Path, content);
            }

            // Print summary
            var compareOnFilepath = new MappedEqualityComparer<File, Filepath>(x => x.Path);

            view.Out(new SummaryViewComponent(
                newFiles: filesToDownload.Except(filesOnClient, compareOnFilepath).ToList(),
                changedFiles: filesToDownload.Intersect(filesOnClient, compareOnFilepath).ToList(),
                sentFiles: filesToUpload));
        }
    }
}
