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
        [Fact]
        public void NoFilesOnClientOrService()
        {
        }
    }
}
