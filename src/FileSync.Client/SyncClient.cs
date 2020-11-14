using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private readonly IDirectoryFactory directoryFactory;
        private readonly IFileHasher fileHasher;
        private readonly IFileServiceApi fileService;

        public SyncClient(
            ITextView view,
            IDirectoryFactory directoryFactory,
            IFileHasher fileHasher,
            IFileServiceApi fileService)
        {
            this.view = view;
            this.directoryFactory = directoryFactory;
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

            var uploadResults = await Task.WhenAll(filesToUpload.Select(file =>
                Result.TryAsync(async () =>
                {
                    var dirname = Path.GetDirectoryName(file.RelativePath.ToString());
                    if (dirname is null)
                    {
                        throw new InvalidOperationException($"{file.RelativePath} does not have a parent directory");
                    }

                    var directory = directoryFactory.Create(new SystemFilepath(dirname));

                    var basename = Path.GetFileName(file.RelativePath.ToString());
                    var content = await directory.ReadFileAsync(basename);

                    await fileService.PutFileContentAsync(file.RelativePath, content);
                    return file;
                })
                .CatchAsync((HttpRequestException e) =>
                {
                    view.Error(new LineViewComponent($"Error uploading file {file.RelativePath}. {e.Message}"));
                    return Task.FromResult(file);
                })));

            // Download file content from the service
            var filesToDownload = compareFiles.FilesToDownload().ToList();
            view.Verbose(new FileListViewComponent("Files to download:", filesToDownload));

            var downloadResults = await Task.WhenAll(filesToDownload.Select(file =>
                Result.TryAsync(async () =>
                {
                    var dirname = Path.GetDirectoryName(file.RelativePath.ToString());
                    if (dirname is null)
                    {
                        throw new InvalidOperationException($"{file.RelativePath} does not have a parent directory");
                    }

                    var directory = directoryFactory.Create(new SystemFilepath(dirname));

                    var basename = Path.GetFileName(file.RelativePath.ToString());
                    var content = await fileService.GetFileContentAsync(file);

                    await directory.WriteFileAsync(basename, content);
                    return file;
                })
                .CatchAsync((HttpRequestException e) =>
                {
                    view.Error(new LineViewComponent($"Error downloading file {file.RelativePath}. {e.Message}"));
                    return Task.FromResult(file);
                })));

            // Print summary
            var compareOnFilepath = new MappedEqualityComparer<FileSyncFile, ForwardSlashFilepath>(x => x.RelativePath);
            var downloadedFiles = downloadResults
                .Successes()
                .ToList();

            view.Out(new SummaryViewComponent(
                sentFiles: uploadResults.Successes(),
                newFiles: downloadedFiles.Except(filesOnClient, compareOnFilepath),
                changedFiles: downloadedFiles.Intersect(filesOnClient, compareOnFilepath)));

            if (uploadResults.Failures().Any())
            {
                view.Error(new FileListViewComponent("Failed to upload some files:", uploadResults.Failures()));
            }

            if (downloadResults.Failures().Any())
            {
                view.Error(new FileListViewComponent("Failed to download some files:", downloadResults.Failures()));
            }
        }

        private IEnumerable<FileSyncFile> GetAllFilesOnClient(SystemFilepath currentDirectory)
        {
            var directory = directoryFactory.Create(currentDirectory);
            foreach (var file in directory.GetFiles())
            {
                yield return FileSyncFile.FromFileInfo(file, currentDirectory);
            }

            foreach (var subdirectory in directory.GetSubdirectories())
            {
                var fullPath = currentDirectory.Combine(subdirectory.Name);
                foreach (var file in GetAllFilesOnClient(fullPath))
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
