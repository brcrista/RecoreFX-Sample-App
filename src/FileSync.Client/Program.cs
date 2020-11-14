using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Recore;

using FileSync.Client.UI;
using FileSync.Common;

namespace FileSync.Client
{
    static class Program
    {
        static void ConfigureServices(IServiceCollection services)
        {
            var currentDirectory = new SystemFilepath(Directory.GetCurrentDirectory());

            var httpClient = new HttpClient
            {
                BaseAddress = new AbsoluteUri("http://localhost:5000/")
            };

            services
                .AddSingleton<IDirectoryFactory>(new DirectoryFactory(currentDirectory))
                .AddSingleton<IFileHasher, FileHasher>()
                .AddSingleton<ITextView>(new ConsoleView { IsVerbose = true })
                .AddSingleton(httpClient)
                .AddSingleton<IFileServiceApi, FileServiceHttpClient>()
                .AddSingleton<SyncClient>();
        }

        static async Task<int> Main()
        {
            try
            {
                var syncClient = Pipeline.Of(new ServiceCollection())
                    .Then(ConfigureServices)
                    .Then(x => x.BuildServiceProvider())
                    .Then(x => x.GetService<SyncClient>())
                    .Result;

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
