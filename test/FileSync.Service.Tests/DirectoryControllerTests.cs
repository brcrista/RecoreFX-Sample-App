using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Recore;
using Recore.Collections.Generic;
using Recore.Security.Cryptography;
using Xunit;

using FileSync.Common;
using FileSync.Common.ApiModels;
using FileSync.Service.Controllers;
using System.Linq;

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
        public void GetListingRootDirectoryFilesOnly()
        {
            var fileStore = new Mock<IFileStore>();
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

            var expected = new DirectoryListing[]
            {
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

            var actual = controller.GetListing().ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetListingSubdirectoryFilesOnly()
        {
        }

        [Fact]
        public void GetListingEmptyDirectory()
        {
        }
    }
}
