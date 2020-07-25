using System.Collections.Generic;

using FileSync.Common.ApiModels;

namespace FileSync.Client.UI
{
    sealed class SummaryViewComponent : IConsoleViewComponent
    {
        private readonly IEnumerable<FileListViewComponent> fileListViews;

        public SummaryViewComponent(
            IReadOnlyList<File> newFiles,
            IReadOnlyList<File> changedFiles,
            IReadOnlyList<File> sentFiles)
        {
            fileListViews = new[]
            {
                new FileListViewComponent($"New files: {newFiles.Count}", newFiles),
                new FileListViewComponent($"Changed files: {changedFiles.Count}", changedFiles),
                new FileListViewComponent($"Sent files: {sentFiles.Count}", sentFiles)
            };
        }

        public IEnumerable<string> GetLines()
        {
            yield return "===== Summary =====";
            yield return "";
            foreach (var view in fileListViews)
            {
                foreach (var line in view.GetLines())
                {
                    yield return line;
                }
            }
        }
    }
}
