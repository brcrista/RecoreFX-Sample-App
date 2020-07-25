using FileSync.Common.ApiModels;

namespace FileSync.Client
{
    public sealed class Conflict
    {
        public File ClientFile { get; set; }
        public File ServerFile { get; set; }
        public ChosenVersion ChosenVersion { get; set; }
    }

    public enum ChosenVersion { Client, Server }
}
