using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

                await foreach (var logMessage in syncClient.RunAsync())
                {
                    var textWriter = logMessage.Level switch
                    {
                        LogLevel.Error => Console.Error,
                        _ => Console.Out
                    };

                    textWriter.WriteLine(logMessage.Message);
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
