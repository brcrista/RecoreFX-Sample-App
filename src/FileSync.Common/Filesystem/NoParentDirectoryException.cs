using System;

namespace FileSync.Common.Filesystem
{
    public class NoParentDirectoryException : Exception
    {
        public NoParentDirectoryException(string directory)
            : base($"{directory} does not have a parent directory")
        {
        }
    }
}
