using System.IO;
using System.Linq;
using Recore;
using Xunit;

using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Tests.SharedMocks;

namespace FileSync.Service.Tests
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    public class DirectoryListingServiceTests
    {
        [Fact]
        public void GetListingRootDirectory()
        {
            var fileStore = FileStoreMock.Mock(
                new[]
                {
                    new DirectoryInfo("directory")
                },
                new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var listingService = new DirectoryListingService(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = listingService.GetListing(new SystemFilepath(".")).ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory(new ForwardSlashFilepath("./directory"), "api/v1/listing?path=./directory"),
                new FileSyncFile(new ForwardSlashFilepath("./hello.txt"), FileStoreMock.DefaultFileTimestamp)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./hello.txt"
                },
                new FileSyncFile(new ForwardSlashFilepath("./world.txt"), FileStoreMock.DefaultFileTimestamp)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingSubdirectory()
        {
            var fileStore = FileStoreMock.Mock(
                new[]
                {
                    new DirectoryInfo("directory")
                },
                new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var listingService = new DirectoryListingService(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = listingService.GetListing(new SystemFilepath("./subdirectory")).ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory(new ForwardSlashFilepath("./subdirectory/directory"), "api/v1/listing?path=./subdirectory/directory"),
                new FileSyncFile(new ForwardSlashFilepath("./subdirectory/hello.txt"), FileStoreMock.DefaultFileTimestamp)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/hello.txt"
                },
                new FileSyncFile(new ForwardSlashFilepath("./subdirectory/world.txt"), FileStoreMock.DefaultFileTimestamp)
                {
                    Sha1 = FileHasherMock.EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingEmptyDirectory()
        {
            var fileStore = FileStoreMock.Mock(
                Enumerable.Empty<DirectoryInfo>(),
                Enumerable.Empty<FileInfo>());

            var listingService = new DirectoryListingService(
                FileStoreMock.MockFactory(fileStore).Object,
                FileHasherMock.Mock().Object);

            var actual = listingService.GetListing(new SystemFilepath("./empty-directory"));
            var expected = Enumerable.Empty<DirectoryListing>();

            Assert.Equal(expected, actual);
        }
    }
}
