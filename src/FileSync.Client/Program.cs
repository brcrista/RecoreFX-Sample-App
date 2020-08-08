using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Recore;

using FileSync.Client.UI;
using FileSync.Common;

namespace FileSync.Client
{
    static class Program
    {
        static async Task<int> Main()
        {
            try
            {
                var fileStoreFactory = Pipeline.Of(Directory.GetCurrentDirectory())
                    .Then(x => new Filepath(x))
                    .Then(x => new FileStoreFactory(x))
                    .Result;

                // Use a single HTTP client for connection pooling
                // (not that it really matters in this application)
                var httpClient = new HttpClient
                {
                    BaseAddress = new AbsoluteUri("http://localhost:5000/")
                };

                var syncClient = new SyncClient(
                    view: new ConsoleView { IsVerbose = true },
                    fileStoreFactory: fileStoreFactory,
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
