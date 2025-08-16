using Csharp.Commands;
using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Text;

namespace Git.Test
{
    public class LogTest : IClassFixture<InitFixture>
    {
        [Fact]
        public static void Log_CreateCommitAndShowHistory()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            var fileName = "testeLog.txt";
            var fileName2 = "testeLog2.txt";

            var content = "test log-commit";
            var content2 = "test log-commit 2";

            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.WriteAllText(fileName2, content2, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Add.Execute([fileName]);
            var lines = CommitUtils.GetIndexEntries();
            Assert.NotEmpty(lines);

            var commitSha1 = Commit.Execute(["-m", "Teste commit 1"]);
            Assert.True(!string.IsNullOrWhiteSpace(commitSha1));

            Add.Execute([fileName2]);
            lines = CommitUtils.GetIndexEntries();
            Assert.NotEmpty(lines);

            var commit2Sha1 = Commit.Execute(["-m", "Teste commit 2"]);
            Assert.True(!string.IsNullOrWhiteSpace(commit2Sha1));

            var result = Log.Execute();

            Assert.Contains(commitSha1, result[1]);
            Assert.Contains("Teste commit 1", result[1]);

            Assert.Contains(commit2Sha1, result[0]);
            Assert.Contains("Teste commit 2", result[0]);
        }
    }
}
