using Microsoft.Extensions.Logging;

namespace FileSync.Client
{
    public sealed class LogMessage
    {
        public LogLevel Level { get; set; }

        public string Message { get; set; }
    }
}
