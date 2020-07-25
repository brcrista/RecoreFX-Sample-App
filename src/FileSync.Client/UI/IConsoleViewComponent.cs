using System.Collections.Generic;

namespace FileSync.Client.UI
{
    public interface IConsoleViewComponent
    {
        public IEnumerable<string> GetLines();
    }
}
