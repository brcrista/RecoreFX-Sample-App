using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    sealed class Conflict
    {
        public File ClientFile { get; set; }
        public File ServerFile { get; set; }
        public ChosenVersion ChosenVersion { get; set; }
    }

    enum ChosenVersion { Client, Service }
}
