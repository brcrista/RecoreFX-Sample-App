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
        private static void ConfigureServices(IServiceCollection services)
        {
            var currentDirectory = new SystemFilepath(Directory.GetCurrentDirectory());

            // Use a single HTTP client for connection pooling
            // (not that it really matters in this application)
            var httpClient = new HttpClient
            {
                BaseAddress = new AbsoluteUri("http://localhost:5000/")
            };

            services
                .AddSingleton<IFileStoreFactory>(new FileStoreFactory(currentDirectory))
                .AddSingleton<IFileHasher, FileHasher>()
                .AddSingleton<ITextView>(new ConsoleView { IsVerbose = true })
                .AddSingleton<IFileServiceApi>(new FileServiceHttpClient(httpClient))
                .AddSingleton<SyncClient>();
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            return services.BuildServiceProvider();
        }

        static async Task<int> Main()
        {
            try
            {
                var serviceProvider = BuildServiceProvider();

                var syncClient = serviceProvider.GetService<SyncClient>();

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
