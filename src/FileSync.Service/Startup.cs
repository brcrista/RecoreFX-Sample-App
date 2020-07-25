using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recore.Functional;

using FileSync.Common;

namespace FileSync.Service
{
    sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var createFileStore = new Composer<IServiceProvider, string>(_ => Directory.GetCurrentDirectory())
                .Then(x => new Filepath(x))
                .Then(x => new FileSystemStore(x));

            services
                .AddTransient<IFileStore, FileSystemStore>(createFileStore.Func)
                .AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
