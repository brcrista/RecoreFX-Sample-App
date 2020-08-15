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
        private static readonly IEqualityComparer<ITextViewComponent> textViewComponentEqualityComparer = new AnonymousEqualityComparer<ITextViewComponent>(
            (lhs, rhs) => Enumerable.SequenceEqual(lhs.GetLines(), rhs.GetLines()),
            _ => 0);

        [Fact]
        public async Task NoFilesOnClientOrService()
        {
            var textView = new Mock<ITextView>();

            var fileStore = FileStoreMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                Enumerable.Empty<FileInfo>());

            var fileService = new Mock<IFileServiceHttpClient>();

            var client = new SyncClient(
                textView.Object,
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            ITextViewComponent expectedTextViewComponent;
            expectedTextViewComponent = new FileListViewComponent("Files on the client:", Enumerable.Empty<FileSyncFile>());
            textView.Verify(
                x => x.Verbose(It.Is<ITextViewComponent>(
                    x => textViewComponentEqualityComparer.Equals(x, expectedTextViewComponent))),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files on the service:", Enumerable.Empty<FileSyncFile>());
            textView.Verify(
                x => x.Verbose(It.Is<ITextViewComponent>(
                    x => textViewComponentEqualityComparer.Equals(x, expectedTextViewComponent))),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to download:", Enumerable.Empty<FileSyncFile>());
            textView.Verify(
                x => x.Verbose(It.Is<ITextViewComponent>(
                    x => textViewComponentEqualityComparer.Equals(x, expectedTextViewComponent))),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to upload:", Enumerable.Empty<FileSyncFile>());
            textView.Verify(
                x => x.Verbose(It.Is<ITextViewComponent>(
                    x => textViewComponentEqualityComparer.Equals(x, expectedTextViewComponent))),
                Times.Once());

            // Verify IFileStore
            fileStore.Verify(
                x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Never);

            fileStore.Verify(
                x => x.ReadFileAsync(It.IsAny<string>()),
                Times.Never);

            // Verify IFileServiceHttpClient
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<Optional<RelativeUri>>()),
                Times.Once);

            fileService.Verify(
                x => x.PutFileContentAsync(It.IsAny<ForwardSlashFilepath>(), It.IsAny<Stream>()),
                Times.Never);
        }
    }
}
