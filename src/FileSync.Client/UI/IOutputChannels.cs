namespace FileSync.Client.UI
{
    public interface IOutputChannels
    {
        void Verbose(string message);

        void Info(string message);

        void Error(string message);
    }
}
