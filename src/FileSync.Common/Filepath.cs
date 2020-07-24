using Recore;

namespace FileSync.Common
{
    public sealed class Filepath : Of<string>
    {
        public Filepath(string value) => Value = value;
    }
}
