using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using FileSync.Common;
using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class FileServiceHttpClient : IFileServiceHttpClient
    {
        private readonly HttpClient httpClient;
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public FileServiceHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<FileSyncFile>> GetDirectoryListingAsync(Filepath path)
        {
            var body = await httpClient.GetStreamAsync($"api/v1/listing/{path}");
            return await JsonSerializer.DeserializeAsync<IEnumerable<FileSyncFile>>(body, jsonOptions);
        }

        public async Task<Stream> GetFileContentAsync(FileSyncFile file)
        {
            return await file.Content.Switch(
                async x => await httpClient.GetStreamAsync(x),
                () => throw new ArgumentNullException(nameof(file.Content)));
        }

        public async Task PutFileContentAsync(Filepath path, Stream content)
        {
            var response = await httpClient.PutAsync($"api/v1/files/{path}/content", new StreamContent(content));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
