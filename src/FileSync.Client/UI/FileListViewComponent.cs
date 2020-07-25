using System.Collections.Generic;

using FileSync.Common.ApiModels;

namespace FileSync.Client.UI
{
    sealed class FileListViewComponent : IConsoleViewComponent
    {
        private readonly string label;
        private readonly IEnumerable<File> files;

        public FileListViewComponent(string label, IEnumerable<File> files)
        {
            this.label = label;
            this.files = files;
        }

        public IEnumerable<string> GetLines()
        {
            yield return label;
            foreach (var file in files)
            {
                yield return Indent(file.Path.Value);
            }

            // Log a blank line
            yield return string.Empty;
        }

        private static string Indent(string source) => "  " + source;
    }
}
