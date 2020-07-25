namespace FileSync.Client.UI
{
    /// <summary>
    /// Represents a text-based medium for showing information to a user.
    /// </summary>
    interface ITextView
    {
        bool IsVerbose { get; set; }

        void Error(ITextViewComponent component);
        void Info(ITextViewComponent component);
        void Verbose(ITextViewComponent component);
    }
}