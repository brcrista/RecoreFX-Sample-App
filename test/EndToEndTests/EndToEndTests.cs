using System.Diagnostics;
using System.IO;
using Xunit;

namespace EndToEndTests
{
    public class EndToEndTests
    {
        [Fact]
        public void EndToEnd()
        {
            var testDirectory = Path.Combine(
                Path.GetTempPath(),
                "file-sync-client-e2e-test");

            Directory.Delete(testDirectory, recursive: true);
            Directory.CreateDirectory(testDirectory);
            Directory.SetCurrentDirectory(testDirectory);

            Directory.CreateDirectory("client");
            File.WriteAllText(Path.Combine("client", "client-only.txt"), "client-only file");
            File.WriteAllText(Path.Combine("client", "shared.txt"), "shared file");

            Directory.CreateDirectory("service");
            File.WriteAllText(Path.Combine("service", "service-only.txt"), "service-only file");
            File.WriteAllText(Path.Combine("service", "shared.txt"), "shared file");

            using (var setup = new Process())
            {
            }
        }
    }
}
