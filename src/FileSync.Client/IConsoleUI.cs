namespace FileSync.Client
{
    public interface IConsoleUI
    {
        void Verbose(string message);

        void Info(string message);

        void Error(string message);
    }
}
