using Recore;

namespace FileSync.Common
{
    [OfJson(typeof(Filepath), typeof(string))]
    public sealed class Filepath : Of<string>
    {
        public Filepath() { }
        public Filepath(string value) => Value = value;
    }
}
