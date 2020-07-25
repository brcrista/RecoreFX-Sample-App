using System;

using Recore.Linq;

namespace FileSync.Client.UI
{
    public sealed class ConsoleView : ITextView
    {
        public bool IsVerbose { get; set; }

        public void Verbose(ITextViewComponent component)
        {
            if (IsVerbose)
            {
                component.GetLines().ForEach(Console.WriteLine);
            }
        }

        public void Info(ITextViewComponent component)
            => component.GetLines().ForEach(Console.WriteLine);

        public void Error(ITextViewComponent component)
            => component.GetLines().ForEach(Console.Error.WriteLine);
    }
}
