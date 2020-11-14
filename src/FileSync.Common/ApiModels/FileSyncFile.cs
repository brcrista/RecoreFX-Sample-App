using System;
using System.IO;
using Recore;

namespace FileSync.Common.ApiModels
{
    public sealed record FileSyncFile
    {
        public FileSyncFile(ForwardSlashFilepath relativePath, DateTime lastWriteTimeUtc)
        {
            RelativePath = relativePath;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }

        public ForwardSlashFilepath RelativePath { get; }

        public DateTime LastWriteTimeUtc { get; }

        /// <summary>
        /// The SHA1 checksum of the file.
        /// </summary>
        /// <remarks>
        /// On the client, this will be computed lazily.
        /// </remarks>
        public string? Sha1 { get; init; }

        /// <summary>
        /// The service URL to download the file.
        /// </summary>
        /// <remarks>
        /// This is empty for files on the client.
        /// </remarks>
        public string? ContentUrl { get; init; }

        /// <summary>
        /// Converts an instance of <seealso cref="FileInfo"/> to <see cref="FileSyncFile"/>.
        /// </summary>
        /// <remarks>
        /// This overload skips the fields that the client doesn't use.
        /// </remarks>
        public static FileSyncFile FromFileInfo(
            FileInfo fileInfo,
            SystemFilepath parentDirectory)
            => FromFileInfo(fileInfo, parentDirectory, null, null);

        /// <summary>
        /// Converts an instance of <seealso cref="FileInfo"/> to <see cref="FileSyncFile"/>.
        /// </summary>
        /// <remarks>
        /// This overload sets all of the fields.
        /// </remarks>
        public static FileSyncFile FromFileInfo(
            FileInfo fileInfo,
            SystemFilepath parentDirectory,
            IFileHasher? fileHasher,
            RelativeUri? contentEndpoint)
        {
            var systemPath = parentDirectory.Combine(fileInfo.Name);

            var forwardSlashPath = ForwardSlashFilepath
                .FromSystemFilepath(parentDirectory)
                .Combine(fileInfo.Name);

            return new FileSyncFile(forwardSlashPath, fileInfo.LastWriteTimeUtc)
            {
                Sha1 = fileHasher?.HashFile(systemPath).Value,
                ContentUrl = contentEndpoint is null ? null : $"{contentEndpoint}?path={forwardSlashPath}"
            };
        }
    }
}
