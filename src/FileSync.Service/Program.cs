using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FileSync.Service
{
    static class Program
    {
        static void Main(string[] args)
        {
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(Configure)
                .Build()
                .Run();
        }

        private static void Configure(IWebHostBuilder webBuilder)
        {
            webBuilder.UseStartup<Startup>();
        }
    }
}
