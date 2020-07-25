using System.Collections.Generic;

namespace FileSync.Client.UI
{
    /// <summary>
    /// Represents some information to be displayed on an <seealso cref="ITextView"/>.
    /// </summary>
    interface ITextViewComponent
    {
        IEnumerable<string> GetLines();
    }
}
