using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class Conflict
    {
        public FileSyncFile ClientFile { get; }
        public FileSyncFile ServiceFile { get; }
        public ChosenVersion ChosenVersion { get; }

        public Conflict(FileSyncFile clientFile, FileSyncFile serviceFile)
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
