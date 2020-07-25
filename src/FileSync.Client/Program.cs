using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FileSync.Common;
using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    static class Program
    {
        static async Task<int> Main()
        {
            try
            {
                // Check the files in our directory
                var fileStore = new FileStore(new Filepath(Directory.GetCurrentDirectory()));
                var filesOnClient = fileStore.GetFiles().Select(ApiModels.File.FromFileInfo);

                // Call the service to get the files on it
                var fileService = new FileServiceHttpClient();
                var filesOnService = await fileService.GetFileInfoAsync();

                var compareFiles = new CompareFiles(filesOnClient, filesOnService);

                // Print a warning for conflicts
                var conflicts = compareFiles.Conflicts();

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
                Console.WriteLine("===== Summary =====");
                // List new files
                Console.WriteLine($"New files: {filesToDownload.Count}"); // TODO
                // List uploaded files
                Console.WriteLine($"Uploaded files: {filesToUpload.Count}");
                // List modified files
                Console.WriteLine($"Modified files: {filesToDownload.Count}"); // TODO

                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }
    }
}
