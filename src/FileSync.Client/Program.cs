using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Recore;

using FileSync.Common;

namespace FileSync.Client
{
    static class Program
    {
        static async Task<int> Main()
        {
            try
            {
                // Use a single HTTP client for connection pooling
                // (not that it really matters in this application)
                var httpClient = new HttpClient
                {
                    BaseAddress = new AbsoluteUri("http://localhost:5000/")
                };

                var syncClient = new SyncClient(
                    consoleUI: new ConsoleUI(),
                    fileStore: new FileSystemFileStore(new Filepath(Directory.GetCurrentDirectory())),
                    fileService: new FileServiceHttpClient(httpClient));

                await syncClient.RunAsync();
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
