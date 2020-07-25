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
                     fileStore: new FileSystemFileStore(new Filepath(Directory.GetCurrentDirectory())),
                     fileService: new FileServiceHttpClient(httpClient));

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
