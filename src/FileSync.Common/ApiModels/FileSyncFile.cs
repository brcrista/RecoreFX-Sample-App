using System;
using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncFile
    {
        public ForwardSlashFilepath RelativePath { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        /// <summary>
        /// The SHA1 checksum of the file.
        /// </summary>
        /// <remarks>
        /// On the client, this will be computed lazily.
        /// </remarks>
        public Optional<string> Sha1 { get; set; }

        /// <summary>
        /// The service URL to download the file.
        /// </summary>
        /// <remarks>
        /// This is empty for files on the client.
        /// </remarks>
        public Optional<string> ContentUrl { get; set; }

        /// <summary>
        /// Converts an instance of <seealso cref="FileInfo"/> to <see cref="FileSyncFile"/>.
        /// </summary>
        /// <remarks>
        /// This overload skips the fields that the client doesn't use.
        /// </remarks>
        public static FileSyncFile FromFileInfo(
            FileInfo fileInfo,
            SystemFilepath parentDirectory)
            => FromFileInfo(fileInfo, parentDirectory, Optional<IFileHasher>.Empty, Optional<RelativeUri>.Empty);

        /// <summary>
        /// Converts an instance of <seealso cref="FileInfo"/> to <see cref="FileSyncFile"/>.
        /// </summary>
        /// <remarks>
        /// This overload sets all of the fields.
        /// </remarks>
        public static FileSyncFile FromFileInfo(
            FileInfo fileInfo,
            SystemFilepath parentDirectory,
            Optional<IFileHasher> fileHasher,
            Optional<RelativeUri> contentEndpoint)
        {
            var systemPath = parentDirectory.Combine(fileInfo.Name);

            var forwardSlashPath = ForwardSlashFilepath
                .FromSystemFilepath(parentDirectory)
                .Combine(fileInfo.Name);

            return new FileSyncFile
            {
                RelativePath = forwardSlashPath,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                Sha1 = fileHasher.OnValue(hasher => hasher.HashFile(systemPath).Value),
                ContentUrl = contentEndpoint.OnValue(endpoint => $"{endpoint}?path={forwardSlashPath}")
            };
        }
    }
}
