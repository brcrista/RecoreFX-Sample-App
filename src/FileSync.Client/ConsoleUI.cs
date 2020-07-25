using System;

namespace FileSync.Client
{
    public sealed class ConsoleUI : IConsoleUI
    {
        public void Error(string message) => Console.Error.WriteLine(message);

        public void Info(string message) => Console.WriteLine(message);

        public void Verbose(string message) => Console.WriteLine(message);
    }
}
