using System.IO;

namespace FileSync.Common.ApiModels
{
    /// <summary>
    /// A filepath that uses the <c>/</c> character as a separator.
    /// </summary>
    /// <remarks>
    /// This is used in the API models to avoid issues when the client and service
    /// are running on different OSes.
    /// Note that <c>/</c> is not a valid filename character on Windows,
    /// so there's no chance of getting mixed up.
    /// </remarks>
    public sealed record ForwardSlashFilepath
    {
        private readonly string value;

        public ForwardSlashFilepath(string value)
        {
            this.value = value;
        }

        public override string ToString() => value;

        public ForwardSlashFilepath Combine(string other)
            => Combine(new ForwardSlashFilepath(other));

        public ForwardSlashFilepath Combine(ForwardSlashFilepath other)
            => new ForwardSlashFilepath(value + "/" + other);

        public SystemFilepath ToFilepath()
            => new SystemFilepath(value.Replace('/', Path.DirectorySeparatorChar));

        public static ForwardSlashFilepath FromSystemFilepath(SystemFilepath filepath)
            => new ForwardSlashFilepath(filepath.ToString().Replace('\\', '/'));
    }
}
