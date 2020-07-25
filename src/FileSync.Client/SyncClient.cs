using System.Linq;
using System.Threading.Tasks;

using FileSync.Client.UI;
using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    /// <summary>
    /// Implements the high-level logic for the file sync client.
    /// </summary>
    public sealed class SyncClient
    {
        private readonly ConsoleView consoleView;
        private readonly IFileStore fileStore;
        private readonly IFileServiceHttpClient fileService;

        public SyncClient(
            ConsoleView consoleView,
            IFileStore fileStore,
            IFileServiceHttpClient fileService)
        {
            this.consoleView = consoleView;
            this.fileStore = fileStore;
            this.fileService = fileService;
        }

        public async Task RunAsync()
        {
            // Check the files in our directory
            var filesOnClient = fileStore.GetFiles().Select(File.FromFileInfo).ToList();
            consoleView.Verbose(new FileListViewComponent("Files on the client:", filesOnClient));

            // Call the service to get the files on it
            var filesOnService = (await fileService.GetAllFileInfoAsync()).ToList();
            consoleView.Verbose(new FileListViewComponent("Files on the service:", filesOnService));

            var compareFiles = new CompareFiles(filesOnClient, filesOnService);

            // Print a message for conflicts
            consoleView.Info(new ConflictsViewComponent(compareFiles.Conflicts()));

            // Download file content from the service
            var filesToDownload = compareFiles.FilesToDownload().ToList();
            foreach (var file in filesToDownload)
            {
                var content = await fileService.GetFileContentAsync(file);
                await fileStore.WriteFileAsync(file.Path, content);
            }

            // Upload files to the service
            var filesToUpload = compareFiles.FilesToUpload().ToList();
            foreach (var file in filesToUpload)
            {
                var content = await fileStore.ReadFileAsync(file.Path);
                await fileService.PutFileContentAsync(content);
            }

            // Print summary
            consoleView.Info(new SummaryViewComponent(
                newFiles: filesToDownload, // TODO
                changedFiles: filesToDownload, // TODO
                sentFiles: filesToUpload));
        }
    }
}
