using System.Collections.Generic;

using FileSync.Common.ApiModels;

namespace FileSync.Client.UI
{
    sealed class SummaryViewComponent : ITextViewComponent
    {
        private readonly IReadOnlyList<FileListViewComponent> fileListViews;

        public SummaryViewComponent(
            IReadOnlyList<FileSyncFile> sentFiles,
            IReadOnlyList<FileSyncFile> newFiles,
            IReadOnlyList<FileSyncFile> changedFiles)
        {
            fileListViews = new[]
            {
                new FileListViewComponent($"Sent files: {sentFiles.Count}", sentFiles),
                new FileListViewComponent($"New files: {newFiles.Count}", newFiles),
                new FileListViewComponent($"Changed files: {changedFiles.Count}", changedFiles)
            };
        }

        public IEnumerable<string> GetLines()
        {
            yield return string.Empty;
            yield return "===== Summary =====";
            yield return string.Empty;
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
