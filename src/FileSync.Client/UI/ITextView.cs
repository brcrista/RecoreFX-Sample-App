namespace FileSync.Client.UI
{
    /// <summary>
    /// Represents a text-based medium for showing information to a user.
    /// </summary>
    interface ITextView
    {
        bool IsVerbose { get; set; }

        void Verbose(ITextViewComponent component);

        void Out(ITextViewComponent component);

        void Error(ITextViewComponent component);
    }
}