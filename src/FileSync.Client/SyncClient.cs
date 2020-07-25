﻿using System;
using System.Collections.Generic;
using System.Linq;

using FileSync.Common;
using Recore;
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

        public async IAsyncEnumerable<string> RunAsync()
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

                yield return $"'{conflict.ClientFile.Path}' exists on both the client and the server."
                    + $" Choosing the {WhoseFile(conflict)}'s version.";
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
            yield return "===== Summary =====";
            // List new files
            yield return $"New files: {filesToDownload.Count}"; // TODO
            // List uploaded files
            yield return $"Uploaded files: {filesToUpload.Count}";
            // List modified files
            yield return $"Modified files: {filesToDownload.Count}"; // TODO
        }
    }
}
