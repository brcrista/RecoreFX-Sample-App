﻿using System.IO;
using Recore;

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
    [OfJson(typeof(ForwardSlashFilepath), typeof(string))]

    public sealed class ForwardSlashFilepath : Of<string>
    {
        // This constructor needs to exist for deserializing using System.Text.Json.
        public ForwardSlashFilepath() : this(string.Empty)
        {
        }

        public ForwardSlashFilepath(string value) => Value = value;

        public ForwardSlashFilepath Combine(string other)
            => Combine(new ForwardSlashFilepath(other));

        public ForwardSlashFilepath Combine(ForwardSlashFilepath other)
            => new ForwardSlashFilepath(Value + "/" + other);

        public SystemFilepath ToFilepath()
            => new SystemFilepath(Value!.Replace('/', Path.DirectorySeparatorChar));

        public static ForwardSlashFilepath FromSystemFilepath(SystemFilepath filepath)
            => new ForwardSlashFilepath(filepath.Value!.Replace('\\', '/'));
    }
}
