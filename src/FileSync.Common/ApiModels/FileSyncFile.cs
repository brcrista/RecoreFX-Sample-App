using System;
using System.IO;
using System.Security.Cryptography;
using Recore;
using Recore.Security.Cryptography;

namespace FileSync.Common.ApiModels
{
    public sealed class FileSyncFile
    {
        public ForwardSlashFilepath RelativePath { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }

        /// <summary>
        /// The SHA1 checksum of the file.
        /// </summary>
        public Ciphertext<SHA1> Sha1 { get; set; }

        /// <summary>
        /// The service URL to download the file.
        /// </summary>
        /// <remarks>
        /// This is empty for files on the client.
        /// </remarks>
        public Optional<string> ContentUrl { get; set; }

        public static FileSyncFile FromFileInfo(
            FileInfo fileInfo,
            Filepath parentDirectory,
            IFileHasher fileHasher,
            Optional<RelativeUri> contentEndpoint)
        {
            var systemPath = parentDirectory.Combine(fileInfo.Name);

            var forwardSlashPath = ForwardSlashFilepath
                .FromFilepath(parentDirectory)
                .Combine(fileInfo.Name);

            return new FileSyncFile
            {
                RelativePath = forwardSlashPath,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                Sha1 = fileHasher.HashFile(systemPath),
                ContentUrl = contentEndpoint.OnValue(endpoint => $"{endpoint}?path={forwardSlashPath}")
            };
        }
    }
}
