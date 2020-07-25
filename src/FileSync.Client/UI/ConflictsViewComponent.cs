using System;
using System.Collections.Generic;

namespace FileSync.Client.UI
{
    sealed class ConflictsViewComponent : IConsoleViewComponent
    {
        private readonly IEnumerable<Conflict> conflicts;

        public ConflictsViewComponent(IEnumerable<Conflict> conflicts)
        {
            this.conflicts = conflicts;
        }

        public IEnumerable<string> GetLines()
        {
            foreach (var conflict in conflicts)
            {
                yield return $"'{conflict.ClientFile.Path}' exists on both the client and the service."
                    + $" Choosing the {WhoseFile(conflict)}'s version.";
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
