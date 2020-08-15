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
            VerifyTextView(
                textView,
                filesOnClient: Enumerable.Empty<FileSyncFile>(),
                filesOnService: Enumerable.Empty<FileSyncFile>(),
                filesToUpload: Enumerable.Empty<FileSyncFile>(),
                filesToDownload: Enumerable.Empty<FileSyncFile>(),
                conflicts: Enumerable.Empty<Conflict>());

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

        [Fact]
        public async Task DifferentFilesOnClientAndService()
        {
            var textView = new Mock<ITextView>();

            var fileStore = FileStoreMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                new[]
                {
                    new FileInfo("client-only-file-1.txt"),
                    new FileInfo("client-only-file-2.txt")
                });

            var fileService = MockFileServiceApi(new DirectoryListing[]
            {
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./service-only-file-1.txt")
                }
            });

            var client = new SyncClient(
                textView.Object,
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            var filesOnClient = new[]
            {
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./client-only-file-1.txt")
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./client-only-file-2.txt")
                }
            };

            var filesOnService = new[]
            {
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./service-only-file-1.txt")
                }
            };

            VerifyTextView(
                textView,
                filesOnClient: filesOnClient,
                filesOnService: filesOnService,
                filesToUpload: filesOnClient,
                filesToDownload: filesOnService,
                conflicts: Enumerable.Empty<Conflict>());

            // Verify IFileStore
            fileStore.Verify(
                x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Once);

            fileStore.Verify(
                x => x.ReadFileAsync(It.IsAny<string>()),
                Times.Exactly(2));

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<Optional<RelativeUri>>()),
                Times.Once);

            fileService.Verify(
                x => x.PutFileContentAsync(It.IsAny<ForwardSlashFilepath>(), It.IsAny<Stream>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task SameFilesDifferentVersionsOnClientAndService()
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
            //VerifyTextView(
            //    textView,
            //    filesOnClient: filesOnClient,
            //    filesOnService: filesOnService,
            //    filesToUpload: filesOnClient,
            //    filesToDownload: filesOnService,
            //    conflicts: Enumerable.Empty<Conflict>());

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

        [Fact]
        public async Task SameFilesSameVersionsOnClientAndService()
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
            //VerifyTextView(
            //    textView,
            //    filesOnClient: filesOnClient,
            //    filesOnService: filesOnService,
            //    filesToUpload: filesOnClient,
            //    filesToDownload: filesOnService,
            //    conflicts: Enumerable.Empty<Conflict>());

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

        private static Mock<IFileServiceApi> MockFileServiceApi(IEnumerable<DirectoryListing> directoryListing)
        {
            var fileService = new Mock<IFileServiceApi>();
            fileService
                .Setup(x => x.GetDirectoryListingAsync(It.IsAny<Optional<RelativeUri>>()))
                .Returns(Task.FromResult(directoryListing));

            return fileService;
        }

        // Provide a way to check that the ITextView received the correct output
        private static TValue ItIsEqual<TValue>(TValue value, IEqualityComparer<TValue> equalityComparer)
            => It.Is<TValue>(
                x => equalityComparer.Equals(x, value));

        private static void VerifyTextView(
            Mock<ITextView> textView,
            IEnumerable<FileSyncFile> filesOnClient,
            IEnumerable<FileSyncFile> filesOnService,
            IEnumerable<FileSyncFile> filesToUpload,
            IEnumerable<FileSyncFile> filesToDownload,
            IEnumerable<Conflict> conflicts)
        {
            var textViewComponentEqualityComparer = new AnonymousEqualityComparer<ITextViewComponent>(
                (lhs, rhs) => Enumerable.SequenceEqual(lhs.GetLines(), rhs.GetLines()),
                _ => 0);

            ITextViewComponent expectedTextViewComponent;
            var noFiles = Enumerable.Empty<FileSyncFile>();

            expectedTextViewComponent = new FileListViewComponent("Files on the client:", filesOnClient);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files on the service:", filesOnService);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to upload:", filesToUpload);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new FileListViewComponent("Files to download:", filesToDownload);
            textView.Verify(
                x => x.Verbose(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once());

            expectedTextViewComponent = new ConflictsViewComponent(conflicts);
            textView.Verify(
                x => x.Out(ItIsEqual(expectedTextViewComponent, textViewComponentEqualityComparer)),
                Times.Once);

            textView.Verify(
                x => x.Out(It.IsAny<SummaryViewComponent>()),
                Times.Once());

            textView.VerifyNoOtherCalls();
        }
    }
}
