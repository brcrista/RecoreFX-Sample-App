using System.Collections.Generic;
using System.Linq;

using FileSync.Common.ApiModels;

namespace FileSync.Client.UI
{
    sealed class FileListViewComponent : ITextViewComponent
    {
        private readonly string label;
        private readonly IReadOnlyList<File> files;

        public FileListViewComponent(string label, IEnumerable<File> files)
        {
            this.label = label;
            this.files = files.ToArray();
        }

        public IEnumerable<string> GetLines()
        {
            yield return label;
            foreach (var file in files)
            {
                yield return Indent(file.Path);
            }

            yield return string.Empty;
        }

        private static string Indent(string source) => "  " + source;
    }
}
