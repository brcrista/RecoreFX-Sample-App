using System;
using System.Collections.Generic;
using System.IO;
using Moq;

using FileSync.Common;

namespace FileSync.Tests.SharedMocks
{
    public static class DirectoryMock
    {
        /// <summary>
        /// The default value of <see cref="FileInfo.LastWriteTimeUtc"/>.
        /// </summary>
        /// <remarks>
        /// Trying to set <see cref="FileInfo.LastWriteTimeUtc"/> will throw a <see cref="FileNotFoundException"/>.
        /// </remarks>
        public static DateTime DefaultFileTimestamp => new DateTime(year: 1601, month: 1, day: 1);

        public static Mock<IDirectory> Mock(
            IEnumerable<DirectoryInfo> directoryInfos,
            IEnumerable<FileInfo> fileInfos)
        {
            var directory = new Mock<IDirectory>();
            directory
                .Setup(x => x.GetSubdirectories())
                .Returns(directoryInfos);

            directory
                .Setup(x => x.GetFiles())
                .Returns(fileInfos);

            return directory;
        }

        public static Mock<IDirectoryFactory> MockFactory(Mock<IDirectory> directory)
        {
            var directoryFactory = new Mock<IDirectoryFactory>();
            directoryFactory
                .Setup(x => x.Open(It.IsAny<SystemFilepath>()))
                .Returns(directory.Object);

            return directoryFactory;
        }
    }
}
