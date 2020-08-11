using System;
using System.IO;
using System.Linq;
using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Service.Controllers;
using Moq;
using Recore;
using Recore.Security.Cryptography;
using Xunit;

namespace FileSync.Service.Tests
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    public class DirectoryControllerTests
    {
        // The SHA1 hash of the empty string, computed with
        // printf "" | sha1sum | awk '{print $1}' | xxd -r -p | base64
        private static readonly string EmptySha1Hash = "2jmj7l5rSw0yVb/vlWAYkK/YBwk=";

        // Trying to set `LastWriteTimeUtc` on a `FileInfo` object will throw a `FileNotFoundException`.
        private static readonly DateTime DefaultFileTimestamp = new DateTime(year: 1601, month: 1, day: 1);

        [Fact]
        public void GetListingRootDirectory()
        {
            var fileStore = new Mock<IFileStore>();
            fileStore
                .Setup(x => x.GetDirectories())
                .Returns(() => new[]
                {
                    new DirectoryInfo("directory")
                });

            fileStore
                .Setup(x => x.GetFiles())
                .Returns(() => new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var fileStoreFactory = new Mock<IFileStoreFactory>();
            fileStoreFactory
                .Setup(x => x.Create(It.IsAny<SystemFilepath>()))
                .Returns(fileStore.Object);

            var fileHasher = new Mock<IFileHasher>();
            fileHasher
                .Setup(x => x.HashFile(It.IsAny<SystemFilepath>()))
                .Returns(() => Ciphertext.SHA1(plaintext: string.Empty, salt: Array.Empty<byte>()));

            var controller = new DirectoryV1Controller(
                fileStoreFactory.Object,
                fileHasher.Object);

            var actual = controller.GetListing().ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory
                {
                    RelativePath = new ForwardSlashFilepath("./directory"),
                    ListingUrl = "api/v1/listing?path=./directory"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./hello.txt"),
                    LastWriteTimeUtc = DefaultFileTimestamp,
                    Sha1 = EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./hello.txt"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./world.txt"),
                    LastWriteTimeUtc = DefaultFileTimestamp,
                    Sha1 = EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingSubdirectory()
        {
            var fileStore = new Mock<IFileStore>();
            fileStore
                .Setup(x => x.GetDirectories())
                .Returns(() => new[]
                {
                    new DirectoryInfo("directory")
                });

            fileStore
                .Setup(x => x.GetFiles())
                .Returns(() => new[]
                {
                    new FileInfo("hello.txt"),
                    new FileInfo("world.txt")
                });

            var fileStoreFactory = new Mock<IFileStoreFactory>();
            fileStoreFactory
                .Setup(x => x.Create(It.IsAny<SystemFilepath>()))
                .Returns(fileStore.Object);

            var fileHasher = new Mock<IFileHasher>();
            fileHasher
                .Setup(x => x.HashFile(It.IsAny<SystemFilepath>()))
                .Returns(() => Ciphertext.SHA1(plaintext: string.Empty, salt: Array.Empty<byte>()));

            var controller = new DirectoryV1Controller(
                fileStoreFactory.Object,
                fileHasher.Object);

            var actual = controller.GetListing("./subdirectory").ToArray();

            var expected = new DirectoryListing[]
            {
                new FileSyncDirectory
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/directory"),
                    ListingUrl = "api/v1/listing?path=./subdirectory/directory"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/hello.txt"),
                    LastWriteTimeUtc = DefaultFileTimestamp,
                    Sha1 = EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/hello.txt"
                },
                new FileSyncFile
                {
                    RelativePath = new ForwardSlashFilepath("./subdirectory/world.txt"),
                    LastWriteTimeUtc = DefaultFileTimestamp,
                    Sha1 = EmptySha1Hash,
                    ContentUrl = "api/v1/content?path=./subdirectory/world.txt"
                }
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingEmptyDirectory()
        {
            var fileStore = new Mock<IFileStore>();
            fileStore
                .Setup(x => x.GetDirectories())
                .Returns(Enumerable.Empty<DirectoryInfo>());

            fileStore
                .Setup(x => x.GetFiles())
                .Returns(Enumerable.Empty<FileInfo>());

            var fileStoreFactory = new Mock<IFileStoreFactory>();
            fileStoreFactory
                .Setup(x => x.Create(It.IsAny<SystemFilepath>()))
                .Returns(fileStore.Object);

            var fileHasher = new Mock<IFileHasher>();
            fileHasher
                .Setup(x => x.HashFile(It.IsAny<SystemFilepath>()))
                .Returns(() => Ciphertext.SHA1(plaintext: string.Empty, salt: Array.Empty<byte>()));

            var controller = new DirectoryV1Controller(
                fileStoreFactory.Object,
                fileHasher.Object);

            var actual = controller.GetListing("./empty-directory");
            var expected = Enumerable.Empty<DirectoryListing>();

            Assert.Equal(expected, actual);
        }
    }
}
