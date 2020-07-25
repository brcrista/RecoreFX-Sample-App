using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

using FileSync.Common;
using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class SyncClient
    {
        private readonly IFileStore fileStore;
        private readonly IFileServiceHttpClient fileService;

        public SyncClient(
            IFileStore fileStore,
            IFileServiceHttpClient fileService)
        {
            this.fileStore = fileStore;
            this.fileService = fileService;
        }

        public async IAsyncEnumerable<LogMessage> RunAsync()
        {
            // Check the files in our directory
            var filesOnClient = fileStore.GetFiles().Select(ApiModels.File.FromFileInfo);

            // Call the service to get the files on it
            var filesOnService = await fileService.GetAllFileInfoAsync();
            var compareFiles = new CompareFiles(filesOnClient, filesOnService);

            // Print a message for conflicts
            foreach (var conflict in compareFiles.Conflicts())
            {
                static string WhoseFile(Conflict conflict)
                    => conflict.ChosenVersion switch
                    {
                        ChosenVersion.Client => "client",
                        ChosenVersion.Server => "server",
                        _ => throw new InvalidOperationException(conflict.ToString())
                    };

                yield return Info($"'{conflict.ClientFile.Path}' exists on both the client and the server."
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
            yield return Info("===== Summary =====");
            // List new files
            yield return Info($"New files: {filesToDownload.Count}"); // TODO
            // List uploaded files
            yield return Info($"Uploaded files: {filesToUpload.Count}");
            // List modified files
            yield return Info($"Modified files: {filesToDownload.Count}"); // TODO
        }

        private static LogMessage Info(string message) => new LogMessage
        {
            Level = LogLevel.Information,
            Message = message
        };

        private static LogMessage Error(string message) => new LogMessage
        {
            Level = LogLevel.Error,
            Message = message
        };
    }
}
