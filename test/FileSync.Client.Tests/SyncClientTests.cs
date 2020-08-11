using System;
using System.IO;
using System.Linq;
using FileSync.Common;
using FileSync.Common.ApiModels;
using Moq;
using Recore;
using Recore.Security.Cryptography;
using Xunit;

namespace FileSync.Service.Tests
{
    using DirectoryListing = Either<FileSyncDirectory, FileSyncFile>;

    public class SyncClientTests
    {
        // Trying to set `LastWriteTimeUtc` on a `FileInfo` object will throw a `FileNotFoundException`.
        private static readonly DateTime DefaultFileTimestamp = new DateTime(year: 1601, month: 1, day: 1);

        [Fact]
        public void NoFilesOnClientOrService()
        {
        }
    }
}
