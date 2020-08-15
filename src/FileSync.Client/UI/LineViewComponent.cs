using System.Collections.Generic;

namespace FileSync.Client.UI
{
    sealed class LineViewComponent : ITextViewComponent
    {
        private readonly string line;

        public LineViewComponent(string line)
        {
            this.line = line;
        }

        public IEnumerable<string> GetLines()
        {
            yield return line;
        }
    }
}
