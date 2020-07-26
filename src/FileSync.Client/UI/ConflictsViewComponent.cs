using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSync.Client.UI
{
    sealed class ConflictsViewComponent : ITextViewComponent
    {
        private readonly IReadOnlyList<Conflict> conflicts;

        public ConflictsViewComponent(IEnumerable<Conflict> conflicts)
        {
            this.conflicts = conflicts.ToArray();
        }

        public IEnumerable<string> GetLines()
        {
            foreach (var conflict in conflicts)
            {
                yield return $"'{conflict.ClientFile.Path}' exists on both the client and the service."
                    + $" Choosing the {WhoseFile(conflict)}'s version.";
            }

            if (conflicts.Any())
            {
                yield return string.Empty;
            }
        }

        private static string WhoseFile(Conflict conflict)
            => conflict.ChosenVersion switch
            {
                ChosenVersion.Client => "client",
                ChosenVersion.Service => "service",
                _ => throw new InvalidOperationException(conflict.ToString())
            };
    }
}
