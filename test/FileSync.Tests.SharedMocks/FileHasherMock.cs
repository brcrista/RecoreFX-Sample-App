using System;
using Moq;
using Recore.Security.Cryptography;

using FileSync.Common;

namespace FileSync.Tests.SharedMocks
{
    public static class FileHasherMock
    {
        /// <summary>
        /// The SHA1 hash of the empty string.
        /// </summary>
        /// <remarks>
        /// This was computed with
        /// <code>
        /// printf "" | sha1sum | awk '{print $1}' | xxd -r -p | base64
        /// </code>
        /// </remarks>
        public static string EmptySha1Hash => "2jmj7l5rSw0yVb/vlWAYkK/YBwk=";

        public static Mock<IFileHasher> Mock()
        {
            var fileHasher = new Mock<IFileHasher>();
            fileHasher
                .Setup(x => x.HashFile(It.IsAny<SystemFilepath>()))
                .Returns(() => Ciphertext.SHA1(plaintext: string.Empty, salt: Array.Empty<byte>()));

            return fileHasher;
        }
    }
}
