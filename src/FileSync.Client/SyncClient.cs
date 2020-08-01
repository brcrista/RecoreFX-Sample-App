using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Recore.Collections.Generic;

using FileSync.Client.UI;
using FileSync.Common;
using FileSync.Common.ApiModels;
using System.Runtime.InteropServices.ComTypes;

namespace FileSync.Client
{
    /// <summary>
    /// Implements the high-level logic for the file sync client.
    /// </summary>
    sealed class SyncClient
    {
        private readonly ITextView view;
        private readonly FileStoreFactory fileStoreFactory;
        private readonly IFileServiceHttpClient fileService;

        public SyncClient(
            ITextView view,
            FileStoreFactory fileStoreFactory,
            IFileServiceHttpClient fileService)
        {
            this.view = view;
            this.fileStoreFactory = fileStoreFactory;
            this.fileService = fileService;
        }

        public async Task RunAsync()
        {
            var fileStore = fileStoreFactory.Create(new Filepath("."));

            // Get the files on the client
            var filesOnClient = GetAllFilesOnClient(fileStore).ToList();
            view.Verbose(new FileListViewComponent("Files on the client:", filesOnClient));

            // Call the service to get the files on it
            var filesOnService = (await GetAllFilesOnService()).ToList();
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
                await fileStore.WriteFileAsync(file.RelativePath, content);
            }

            // Upload files to the service
            var filesToUpload = compareFiles.FilesToUpload().ToList();
            view.Verbose(new FileListViewComponent("Files to upload:", filesToUpload));

            foreach (var file in filesToUpload)
            {
                var content = await fileStore.ReadFileAsync(file.RelativePath);
                await fileService.PutFileContentAsync(file.RelativePath, content);
            }

            // Print summary
            var compareOnFilepath = new MappedEqualityComparer<FileSyncFile, Filepath>(x => x.RelativePath);

            view.Out(new SummaryViewComponent(
                newFiles: filesToDownload.Except(filesOnClient, compareOnFilepath).ToList(),
                changedFiles: filesToDownload.Intersect(filesOnClient, compareOnFilepath).ToList(),
                sentFiles: filesToUpload));
        }

        private IEnumerable<FileSyncFile> GetAllFilesOnClient(IFileStore fileStore)
        {
            // TODO this doesn't prepend the path
            //foreach (var file in fileStore.GetFiles())
            //{
            //    yield return file;
            //}
            return Directory.EnumerateFiles(".", searchPattern: "*", enumerationOptions: new EnumerationOptions
            {
                RecurseSubdirectories = true
            })
            .Select(file => new FileInfo(file))
            .Select(file => FileSyncFile.FromFileInfo(file, new Filepath(".")));
        }

        private async Task<IEnumerable<FileSyncFile>> GetAllFilesOnService()
        {
            // TODO recurse
            return await fileService.GetDirectoryListingAsync(new Filepath("."));
        }
    }
}
