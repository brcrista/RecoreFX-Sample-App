using System.IO;
using Recore;

namespace FileSync.Common
{
    /// <summary>
    /// A filepath that uses the current platform's directory separator.
    /// </summary>
    [OfJson(typeof(SystemFilepath), typeof(string))]
    public sealed class SystemFilepath : Of<string>
    {
        // This constructor needs to exist for deserializing using System.Text.Json.
        public SystemFilepath() : this(string.Empty)
        {
        }

        public SystemFilepath(string value) => Value = value;

        public SystemFilepath Combine(string other) => Combine(new SystemFilepath(other));

        public SystemFilepath Combine(SystemFilepath other) => new SystemFilepath(Path.Combine(Value!, other!));
    }
}
