using System;

using Recore.Linq;

namespace FileSync.Client.UI
{
    sealed class ConsoleView : ITextView
    {
        public bool IsVerbose { get; set; }

        public void Verbose(ITextViewComponent component)
        {
            if (IsVerbose)
            {
                component.GetLines().ForEach(Console.WriteLine);
            }
        }

        public void Out(ITextViewComponent component)
            => component.GetLines().ForEach(Console.WriteLine);

        public void Error(ITextViewComponent component)
            => component.GetLines().ForEach(Console.Error.WriteLine);
    }
}
