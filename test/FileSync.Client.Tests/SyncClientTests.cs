using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Recore;
using Recore.Collections.Generic;
using Recore.Security.Cryptography;
using Xunit;

using FileSync.Client.UI;
using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Tests.SharedMocks;

namespace FileSync.Client.Tests
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    public class SyncClientTests
    {
        // Provide a way to check that the ITextView received the correct output
        private static readonly IEqualityComparer<ITextViewComponent> textViewComponentEqualityComparer = new AnonymousEqualityComparer<ITextViewComponent>(
            (lhs, rhs) => Enumerable.SequenceEqual(lhs.GetLines(), rhs.GetLines()),
            _ => 0);

        private static TValue ItIsEqual<TValue>(TValue value, IEqualityComparer<TValue> equalityComparer)
            => It.Is<TValue>(
                x => equalityComparer.Equals(x, value));

        [Fact]
        public async Task NoFilesOnClientOrService()
        {
            var textView = new Mock<ITextView>();

            var fileStore = FileStoreMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                Enumerable.Empty<FileInfo>());

            var fileService = new Mock<IFileServiceApi>();

            var client = new SyncClient(
                textView.Object,
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            ITextViewComponent expectedTextViewComponent;
            var noFiles = Enumerable.Empty<FileSyncFile>();

            expectedTextViewComponent = new FileListViewComponent("Files on the client:", noFiles);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files on the service:", noFiles);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to download:", noFiles);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to upload:", noFiles);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            // Verify IFileStore
            fileStore.Verify(
                x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Never);

            fileStore.Verify(
                x => x.ReadFileAsync(It.IsAny<string>()),
                Times.Never);

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<Optional<RelativeUri>>()),
                Times.Once);

            fileService.Verify(
                x => x.PutFileContentAsync(It.IsAny<ForwardSlashFilepath>(), It.IsAny<Stream>()),
                Times.Never);
        }
    }
}
