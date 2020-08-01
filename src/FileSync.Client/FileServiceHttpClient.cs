using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Recore;

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

        public async Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync()
            => await GetDirectoryListingAsync(new RelativeUri("api/v1/listing"));

        public async Task<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>> GetDirectoryListingAsync(RelativeUri listingUri)
        {
            var body = await httpClient.GetStreamAsync(listingUri);
            return await JsonSerializer.DeserializeAsync<IEnumerable<Either<FileSyncDirectory, FileSyncFile>>>(body, jsonOptions);
        }

        public async Task<Stream> GetFileContentAsync(FileSyncFile file)
        {
            return await file.ContentUrl.Switch(
                async x => await httpClient.GetStreamAsync(x),
                () => throw new ArgumentNullException(nameof(file.ContentUrl)));
        }

        public async Task PutFileContentAsync(Filepath path, Stream content)
        {
            var response = await httpClient.PutAsync($"api/v1/content/{path}", new StreamContent(content));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
