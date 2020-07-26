using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class Conflict
    {
        public File ClientFile { get; }
        public File ServiceFile { get; }
        public ChosenVersion ChosenVersion { get; }

        public Conflict(File clientFile, File serviceFile)
        {
            ClientFile = clientFile;
            ServiceFile = serviceFile;
            ChosenVersion = serviceFile.LastWriteTimeUtc > clientFile.LastWriteTimeUtc
                ? ChosenVersion.Service
                : ChosenVersion.Client;
        }
    }

    enum ChosenVersion { Client, Service }
}
