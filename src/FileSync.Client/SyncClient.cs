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
            var filesOnClient = fileStore.GetFiles().Select(ApiModels.File.FromFileInfo).ToList();
            yield return Verbose("Files on the client:");
            foreach (var message in LogFiles(Verbose, filesOnClient))
            {
                yield return message;
            }

            // Call the service to get the files on it
            var filesOnService = await fileService.GetAllFileInfoAsync();
            yield return Verbose("Files on the service:");
            foreach (var message in LogFiles(Verbose, filesOnClient))
            {
                yield return message;
            }

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

                yield return Info($"'{conflict.ClientFile.Path}' exists on both the client and the service."
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
            foreach (var message in LogFiles(Info, filesToDownload))
            {
                yield return message;
            }

            // List uploaded files
            yield return Info($"Uploaded files: {filesToUpload.Count}");
            foreach (var message in LogFiles(Info, filesToUpload))
            {
                yield return message;
            }

            // List modified files
            yield return Info($"Modified files: {filesToDownload.Count}"); // TODO
            foreach (var message in LogFiles(Info, filesToDownload))
            {
                yield return message;
            }
        }

        #region log helpers
        private static LogMessage CreateLogMessage(LogLevel level, string message)
            => new LogMessage { Level = level, Message = message };

        private static LogMessage Verbose(string message)
            => CreateLogMessage(LogLevel.Debug, message);

        private static LogMessage Info(string message)
            => CreateLogMessage(LogLevel.Information, message);

        private static IEnumerable<LogMessage> LogFiles(Func<string, LogMessage> logLevel, IEnumerable<ApiModels.File> files)
        {
            foreach (var file in files)
            {
                yield return logLevel(Indent(file.Path.Value));
            }

            // Log a blank line
            yield return logLevel(string.Empty);
        }

        private static string Indent(string source) => "  " + source;
        #endregion
    }
}
