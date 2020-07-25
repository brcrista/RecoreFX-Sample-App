using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileSync.Common;
using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class SyncClient
    {
        private readonly IConsoleUI consoleUI;
        private readonly IFileStore fileStore;
        private readonly IFileServiceHttpClient fileService;

        public SyncClient(
            IConsoleUI consoleUI,
            IFileStore fileStore,
            IFileServiceHttpClient fileService)
        {
            this.consoleUI = consoleUI;
            this.fileStore = fileStore;
            this.fileService = fileService;
        }

        public async Task RunAsync()
        {
            // Check the files in our directory
            var filesOnClient = fileStore.GetFiles().Select(ApiModels.File.FromFileInfo).ToList();
            ListFiles(consoleUI.Verbose, "Files on the client:", filesOnClient);

            // Call the service to get the files on it
            var filesOnService = (await fileService.GetAllFileInfoAsync()).ToList();
            ListFiles(consoleUI.Verbose, "Files on the service:", filesOnService);

            var compareFiles = new CompareFiles(filesOnClient, filesOnService);

            // Print a message for conflicts
            foreach (var conflict in compareFiles.Conflicts())
            {
                static string WhoseFile(Conflict conflict)
                    => conflict.ChosenVersion switch
                    {
                        ChosenVersion.Client => "client",
                        ChosenVersion.Service => "service",
                        _ => throw new InvalidOperationException(conflict.ToString())
                    };

                consoleUI.Info($"'{conflict.ClientFile.Path}' exists on both the client and the service."
                    + $" Choosing the {WhoseFile(conflict)}'s version.");
            }

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
            consoleUI.Info("===== Summary =====");

            // List new files
            ListFiles(consoleUI.Info, $"New files: {filesToDownload.Count}", filesToDownload); // TODO

            // List uploaded files
            ListFiles(consoleUI.Info, $"Uploaded files: {filesToUpload.Count}", filesToUpload);

            // List modified files
            ListFiles(consoleUI.Info, $"Modified files: {filesToDownload.Count}", filesToDownload); // TODO
        }

        private static void ListFiles(Action<string> channel, string message, IEnumerable<ApiModels.File> files)
        {
            static string Indent(string source) => "  " + source;

            channel(message);
            foreach (var file in files)
            {
                channel(Indent(file.Path.Value));
            }

            // Log a blank line
            channel(string.Empty);
        }
    }
}
