using System.Security.Cryptography;
using Recore.Security.Cryptography;

namespace FileSync.Common
{
    public interface IFileHasher
    {
        Ciphertext<SHA1> HashFile(SystemFilepath filepath);
    }
}
