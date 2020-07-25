using System;

using Recore.Linq;

namespace FileSync.Client.UI
{
    public sealed class ConsoleView
    {
        public bool IsVerbose { get; set; }

        public void Verbose(IConsoleViewComponent component)
        {
            if (IsVerbose)
            {
                component.GetLines().ForEach(Console.WriteLine);
            }
        }

        public void Info(IConsoleViewComponent component)
            => component.GetLines().ForEach(Console.WriteLine);

        public void Error(IConsoleViewComponent component)
            => component.GetLines().ForEach(Console.Error.WriteLine);
    }
}
