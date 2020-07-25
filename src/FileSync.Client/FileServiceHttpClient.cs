﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using ApiModels = FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class FileServiceHttpClient : IFileServiceHttpClient
    {
        private readonly HttpClient httpClient;

        public FileServiceHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<ApiModels.File>> GetFileInfosAsync()
        {
            var response = await httpClient.GetAsync("api/v1/files");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            //var body = await response.Content.ReadAsStreamAsync();
            //return await JsonSerializer.DeserializeAsync<IEnumerable<ApiModels.File>>(body);

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ApiModels.File>>(body);
        }

        public async Task<Stream> GetFileContentAsync(ApiModels.File file)
        {
            var response = await httpClient.GetAsync(file.Links["self"].Href);
            return await response.Content.ReadAsStreamAsync();
        }

        public Task<ApiModels.File> PutFileContentAsync(Stream content)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
