using Recore;

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

            ChosenVersion = Func.Invoke(() =>
            {
                bool serviceVersionIsNewer = serviceFile.LastWriteTimeUtc > clientFile.LastWriteTimeUtc;

                if (serviceVersionIsNewer)
                {
                    return ChosenVersion.Service;
                }
                else
                {
                    return ChosenVersion.Client;
                }
            });
        }
    }

    enum ChosenVersion { Client, Service }
}
