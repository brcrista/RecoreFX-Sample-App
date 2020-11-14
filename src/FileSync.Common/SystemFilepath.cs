using System.IO;

namespace FileSync.Common
{
    /// <summary>
    /// A filepath that uses the current platform's directory separator.
    /// </summary>
    public sealed record SystemFilepath
    {
        private readonly string value;

        public SystemFilepath(string value)
        {
            this.value = value;
        }

        public override string ToString() => value;

        public SystemFilepath Combine(string other) => Combine(new SystemFilepath(other));

        public SystemFilepath Combine(SystemFilepath other) => new SystemFilepath(Path.Combine(value, other.value));

        /// <summary>
        /// Separates a filepath into a parent directory and the trailing directory or file name, plus the extension.
        /// </summary>
        public (SystemFilepath dirname, string basename) Split()
        {
            var dirname = Path.GetDirectoryName(value);
            if (dirname is null)
            {
                throw new NoParentDirectoryException(value);
            }

            return (new SystemFilepath(dirname), Path.GetFileName(value));
        }
    }
}
