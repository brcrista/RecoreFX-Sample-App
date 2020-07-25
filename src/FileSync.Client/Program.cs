using System;
using System.IO;
using System.Threading.Tasks;

using FileSync.Common;

namespace FileSync.Client
{
    static class Program
    {
        static async Task<int> Main()
        {
            try
            {
                var syncClient = new SyncClient(
                     fileStore: new FileStore(new Filepath(Directory.GetCurrentDirectory())),
                     fileService: new FileServiceHttpClient());

                await foreach (var resultLine in syncClient.RunAsync())
                {
                    Console.WriteLine(resultLine);
                }

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
