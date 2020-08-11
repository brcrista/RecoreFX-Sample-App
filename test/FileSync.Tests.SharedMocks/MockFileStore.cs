using System.Collections.Generic;
using System.IO;
using Moq;

using FileSync.Common;

namespace FileSync.Tests.SharedMocks
{
    public static class MockFileStore
    {
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
