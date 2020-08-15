using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Recore;
using Recore.Text.Json.Serialization.Converters;

using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    sealed class FileServiceHttpClient : IFileServiceApi
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;

        public FileServiceHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;

            jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // FileSyncDirectory and FileSyncFile are both POCOs
            jsonOptions.Converters.Add(
                new OverrideEitherConverter<FileSyncDirectory, FileSyncFile>(
                    deserializeAsLeft: json => json.TryGetProperty("listingUrl", out JsonElement _)));
        }

        public async Task<IEnumerable<DirectoryListing>> GetDirectoryListingAsync(Optional<RelativeUri> listingUri)
            => await Pipeline.Of(listingUri)
                .Then(uri => uri.ValueOr(new RelativeUri("api/v1/listing")))
                .Then(uri => httpClient.GetStreamAsync(uri))
                .Then(async body => await JsonSerializer.DeserializeAsync<IEnumerable<DirectoryListing>>(await body, jsonOptions))
                .Result;

        public async Task<Stream> GetFileContentAsync(FileSyncFile file)
            => await file.ContentUrl.Switch(
                async x => await httpClient.GetStreamAsync(x),
                () => throw new ArgumentNullException(nameof(file.ContentUrl)));

        public async Task PutFileContentAsync(ForwardSlashFilepath path, Stream content)
        {
            var response = await httpClient.PutAsync($"api/v1/content?path={path}", new StreamContent(content));
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
    }
}
