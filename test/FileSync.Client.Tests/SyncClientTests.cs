using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Recore;
using Recore.Collections.Generic;
using Xunit;

using FileSync.Client.UI;
using FileSync.Common.ApiModels;
using FileSync.Common.Filesystem;
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

            var directory = DirectoryMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                Enumerable.Empty<FileInfo>());

            var fileService = new Mock<IFileServiceApi>();

            var client = new SyncClient(
                textView.Object,
                DirectoryMock.MockFactory(directory).Object,
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
            directory.Verify(
                x => x.GetFiles(),
                Times.Once);

            directory.Verify(
                x => x.GetSubdirectories(),
                Times.Once);

            directory.VerifyNoOtherCalls();

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()),
                Times.Once);

            fileService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DifferentFilesOnClientAndService()
        {
            var textView = new Mock<ITextView>();

            var directory = DirectoryMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                new[]
                {
                    new FileInfo("client-only-file-1.txt"),
                    new FileInfo("client-only-file-2.txt")
                });

            var fileService = MockFileServiceApi(new DirectoryListing[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./service-only-file-1.txt"), DateTime.UtcNow)
            });

            var client = new SyncClient(
                textView.Object,
                DirectoryMock.MockFactory(directory).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            var filesOnClient = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./client-only-file-1.txt"), DateTime.UtcNow),
                new FileSyncFile(new ForwardSlashFilepath("./client-only-file-2.txt"), DateTime.UtcNow)
            };

            var filesOnService = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./service-only-file-1.txt"), DateTime.UtcNow)
            };

            VerifyTextView(
                textView,
                filesOnClient: filesOnClient,
                filesOnService: filesOnService,
                filesToUpload: filesOnClient,
                filesToDownload: filesOnService,
                conflicts: Enumerable.Empty<Conflict>());

            // Verify IFileStore
            directory.Verify(
                x => x.GetFiles(),
                Times.Once);

            directory.Verify(
                x => x.GetSubdirectories(),
                Times.Once);

            directory.Verify(
                x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Once);

            directory.Verify(
                x => x.ReadFileAsync(It.IsAny<string>()),
                Times.Exactly(2));

            directory.VerifyNoOtherCalls();

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()),
                Times.Once);

            fileService.Verify(
                x => x.PutFileContentAsync(It.IsAny<ForwardSlashFilepath>(), It.IsAny<Stream>()),
                Times.Exactly(2));

            fileService.Verify(
                x => x.GetFileContentAsync(It.IsAny<FileSyncFile>()),
                Times.Once);

            fileService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SameFilesSameVersionsOnClientAndService()
        {
            var textView = new Mock<ITextView>();

            var directory = DirectoryMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                new[]
                {
                    new FileInfo("shared-file.txt"),
                });

            var fileService = MockFileServiceApi(new DirectoryListing[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash
                }
            });

            var client = new SyncClient(
                textView.Object,
                DirectoryMock.MockFactory(directory).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            var filesOnClient = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
            };

            var filesOnService = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash
                }
            };

            VerifyTextView(
                textView,
                filesOnClient: filesOnClient,
                filesOnService: filesOnService,
                filesToUpload: Enumerable.Empty<FileSyncFile>(),
                filesToDownload: Enumerable.Empty<FileSyncFile>(),
                conflicts: Enumerable.Empty<Conflict>());

            // Verify IFileStore
            directory.Verify(
                x => x.GetFiles(),
                Times.Once);

            directory.Verify(
                x => x.GetSubdirectories(),
                Times.Once);

            directory.VerifyNoOtherCalls();

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()),
                Times.Once);

            fileService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SameFilesDifferentVersionsOnClientAndService()
        {
            var textView = new Mock<ITextView>();

            var directory = DirectoryMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                new[]
                {
                    new FileInfo("shared-file.txt")
                });

            var fileService = MockFileServiceApi(new DirectoryListing[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
                {
                    Sha1 = "1234"
                }
            });

            var client = new SyncClient(
                textView.Object,
                DirectoryMock.MockFactory(directory).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            var filesOnClient = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
            };

            var filesOnService = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./shared-file.txt"), DateTime.UtcNow)
            };

            var conflicts = new[]
            {
                new Conflict(
                    clientFile: filesOnClient[0],
                    serviceFile: filesOnService[0])
            };

            VerifyTextView(
                textView,
                filesOnClient: filesOnClient,
                filesOnService: filesOnService,
                filesToUpload: Enumerable.Empty<FileSyncFile>(),
                filesToDownload: filesOnService,
                conflicts: conflicts);

            // Verify IFileStore
            directory.Verify(
                x => x.GetFiles(),
                Times.Once);

            directory.Verify(
                x => x.GetSubdirectories(),
                Times.Once);

            directory.Verify(
                x => x.WriteFileAsync(It.IsAny<string>(), It.IsAny<Stream>()),
                Times.Once);

            directory.VerifyNoOtherCalls();

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()),
                Times.Once);

            fileService.Verify(
                x => x.GetFileContentAsync(It.IsAny<FileSyncFile>()),
                Times.Once);

            fileService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ContinuesOnFailedDownload()
        {
            var textView = new Mock<ITextView>();

            var directory = DirectoryMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                new[]
                {
                    new FileInfo("client-only-file-1.txt"),
                    new FileInfo("client-only-file-2.txt")
                });

            var fileService = MockFileServiceApi(new DirectoryListing[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./service-only-file-1.txt"), DateTime.UtcNow)
            });

            fileService
                .Setup(x => x.GetFileContentAsync(It.Is<FileSyncFile>(
                    file => file.RelativePath.ToString() == "./service-only-file-1.txt")))
                .Throws<HttpRequestException>();

            var client = new SyncClient(
                textView.Object,
                DirectoryMock.MockFactory(directory).Object,
                FileHasherMock.Mock().Object,
                fileService.Object);

            await client.RunAsync();

            // Verify ITextView
            var filesOnClient = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./client-only-file-1.txt"), DateTime.UtcNow),
                new FileSyncFile(new ForwardSlashFilepath("./client-only-file-2.txt"), DateTime.UtcNow)
            };

            var filesOnService = new[]
            {
                new FileSyncFile(new ForwardSlashFilepath("./service-only-file-1.txt"), DateTime.UtcNow)
            };

            textView.Verify(
                x => x.Error(It.IsAny<LineViewComponent>()),
                Times.Once());

            textView.Verify(
                x => x.Error(It.IsAny<FileListViewComponent>()),
                Times.Once());

            VerifyTextView(
                textView,
                filesOnClient: filesOnClient,
                filesOnService: filesOnService,
                filesToUpload: filesOnClient,
                filesToDownload: filesOnService,
                conflicts: Enumerable.Empty<Conflict>());

            // Verify IFileStore
            directory.Verify(
                x => x.GetFiles(),
                Times.Once);

            directory.Verify(
                x => x.GetSubdirectories(),
                Times.Once);

            directory.Verify(
                x => x.ReadFileAsync(It.IsAny<string>()),
                Times.Exactly(2));

            directory.VerifyNoOtherCalls();

            // Verify IFileServiceApi
            fileService.Verify(
                x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()),
                Times.Once);

            fileService.Verify(
                x => x.PutFileContentAsync(It.IsAny<ForwardSlashFilepath>(), It.IsAny<Stream>()),
                Times.Exactly(2));

            fileService.Verify(
                x => x.GetFileContentAsync(It.IsAny<FileSyncFile>()),
                Times.Once);

            fileService.VerifyNoOtherCalls();
        }

        private static Mock<IFileServiceApi> MockFileServiceApi(IEnumerable<DirectoryListing> directoryListing)
        {
            var fileService = new Mock<IFileServiceApi>();
            fileService
                .Setup(x => x.GetDirectoryListingAsync(It.IsAny<RelativeUri?>()))
                .Returns(Task.FromResult(directoryListing));

            return fileService;
        }

        // Moq won't let you pass an IEqualityComparer, just a Func
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
                (lhs, rhs) => 
                    lhs is not null
                    && rhs is not null
                    && Enumerable.SequenceEqual(lhs.GetLines(), rhs.GetLines()),
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
