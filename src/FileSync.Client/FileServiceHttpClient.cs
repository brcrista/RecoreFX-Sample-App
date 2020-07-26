using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using FileSync.Common;
using ApiModels = FileSync.Common.ApiModels;

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

        public async Task<IEnumerable<ApiModels.File>> GetAllFileInfoAsync()
        {
            var body = await httpClient.GetStreamAsync("api/v1/files");
            return await JsonSerializer.DeserializeAsync<IEnumerable<ApiModels.File>>(body, jsonOptions);
        }

        public async Task<Stream> GetFileContentAsync(ApiModels.File file)
        {
            return await httpClient.GetStreamAsync(file.Links["content"].Href);
        }

        public async Task PutFileContentAsync(Filepath path, Stream content)
        {
            var response = await httpClient.PutAsync($"api/v1/files/{path.Value}/content", new StreamContent(content));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
