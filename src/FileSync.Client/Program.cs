using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Recore;

using FileSync.Client.UI;
using FileSync.Common;
using FileSync.Common.Filesystem;

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
                var syncClient = new ServiceCollection()
                    .Apply(ConfigureServices)
                    .BuildServiceProvider()
                    .GetService<SyncClient>();

                if (syncClient is null)
                {
                    throw new InvalidOperationException("Dependency injection was not set up correctly.");
                }

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
