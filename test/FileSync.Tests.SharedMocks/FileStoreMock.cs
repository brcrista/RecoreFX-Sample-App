using System;
using System.Collections.Generic;
using System.IO;
using Moq;

using FileSync.Common;

namespace FileSync.Tests.SharedMocks
{
    public static class FileStoreMock
    {
        /// <summary>
        /// The default value of <see cref="FileInfo.LastWriteTimeUtc"/>.
        /// </summary>
        /// <remarks>
        /// Trying to set <see cref="FileInfo.LastWriteTimeUtc"/> will throw a <see cref="FileNotFoundException"/>.
        /// </remarks>
        public static DateTime DefaultFileTimestamp => new DateTime(year: 1601, month: 1, day: 1);

        public static Mock<IFileStore> Mock(
            IEnumerable<DirectoryInfo> directoryInfos,
            IEnumerable<FileInfo> fileInfos)
        {
            var fileStore = new Mock<IFileStore>();
            fileStore
                .Setup(x => x.GetDirectories())
                .Returns(directoryInfos);

            fileStore
                .Setup(x => x.GetFiles())
                .Returns(fileInfos);

            return fileStore;
        }

        public static Mock<IFileStoreFactory> MockFactory(Mock<IFileStore> fileStore)
        {
            var fileStoreFactory = new Mock<IFileStoreFactory>();
            fileStoreFactory
                .Setup(x => x.Create(It.IsAny<SystemFilepath>()))
                .Returns(fileStore.Object);

            return fileStoreFactory;
        }
    }
}
