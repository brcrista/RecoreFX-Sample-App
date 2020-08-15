using System.Collections.Generic;
using System.Linq;

using FileSync.Common.ApiModels;

namespace FileSync.Client.UI
{
    sealed class SummaryViewComponent : ITextViewComponent
    {
        private readonly IReadOnlyList<FileListViewComponent> fileListViews;

        public SummaryViewComponent(
            IEnumerable<FileSyncFile> sentFiles,
            IEnumerable<FileSyncFile> newFiles,
            IEnumerable<FileSyncFile> changedFiles)
        {
            var sentFilesList = sentFiles.ToList();
            var newFilesList = newFiles.ToList();
            var changedFilesList = changedFiles.ToList();

            fileListViews = new[]
            {
                new FileListViewComponent($"Sent files: {sentFilesList.Count}", sentFilesList),
                new FileListViewComponent($"New files: {newFilesList.Count}", newFilesList),
                new FileListViewComponent($"Changed files: {changedFilesList.Count}", changedFilesList)
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
