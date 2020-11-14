using System;

namespace FileSync.Common
{
    public class NoParentDirectoryException : Exception
    {
        public NoParentDirectoryException(string directory)
            : base($"{directory} does not have a parent directory")
        {
        }
    }
}
