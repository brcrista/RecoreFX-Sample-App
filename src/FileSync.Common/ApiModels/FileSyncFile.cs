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

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath, IFileHasher fileHasher)
            => FromFileInfo(fileInfo, relativePath, fileHasher, contentUri: Optional<RelativeUri>.Empty);

        public static FileSyncFile FromFileInfo(FileInfo fileInfo, Filepath relativePath, IFileHasher fileHasher, Optional<RelativeUri> contentUri)
        {
            var systemFilepath = relativePath.Combine(new Filepath(fileInfo.Name));

            var forwardSlashFilepath = ForwardSlashFilepath
                .FromFilepath(relativePath)
                .Combine(new ForwardSlashFilepath(fileInfo.Name));

            return new FileSyncFile
            {
                RelativePath = forwardSlashFilepath,
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc,
                Sha1 = fileHasher.HashFile(systemFilepath),
                ContentUrl = contentUri.OnValue(x => x.ToString())
            };
        }
    }
}
