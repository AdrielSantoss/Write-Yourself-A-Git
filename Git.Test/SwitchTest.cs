using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Diagnostics.Metrics;
using System.Text;
namespace Git.Test
{
    public class SwitchTest : IClassFixture<InitFixture>
    {
        [Theory]
        [InlineData("branchTest2")]
        public void Switch_CreateBranchAndSwitchHead(string branchName)
        {
            var fileName = "testeCommit.txt";
            var content = "test commit";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Add.Execute([fileName]);

            var indexEntries = IndexUtils.GetIndexEntries();
            Assert.NotEmpty(indexEntries);

            var indexEntry = indexEntries[fileName];
            Assert.NotNull(indexEntry);

            var sha1BlobExpected = BlobUtils.GetSha1FromBlob(fileName);
            Assert.Contains(sha1BlobExpected, indexEntries.Values);

            var commitSha1 = Commit.Execute(["-m", "commit test"]);
            Assert.NotNull(commitSha1);

            Branch.Execute([branchName]);

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var branchPath = Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}");
            Assert.True(Path.Exists(branchPath));

            var branchContent = File.ReadAllText(branchPath);
            Assert.Equal(commitSha1, branchContent);

            Switch.Execute([branchName]);

            var head = BranchUtils.GetHead();
            Assert.Equal($"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}", head);
        }
    }
}
