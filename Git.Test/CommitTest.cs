using Csharp.Commands;
using Csharp.Core;
using Csharp.Test.Configs;
using Git.Commands;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Git.Test
{
    public class CommitTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void Commit_CreateCommitAndVerifyTree()
        {
            var fileName = "testeCommit.txt";
            var content = "test commit";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Add.Execute([fileName]);

            var lines = Utils.GetIndexFileContentLines();
            Assert.NotEmpty(lines); 

            var line = lines.FirstOrDefault(line => line.Contains(fileName));
            Assert.NotNull(line);

            var parts = line.Split(" ", 2);
            Assert.NotEmpty(parts);

            var sha1BlobExpected = Utils.GetSha1FromBlob(fileName);
            Assert.Contains(sha1BlobExpected, parts);

            var commitSha1 = Commit.Execute(["-m", "commit test"]);

            var commitData = Utils.GetObjectDataBySha1(commitSha1);
            var nullIndex = Array.IndexOf(commitData, (byte)0);
            var commitContent = Encoding.UTF8.GetString(commitData[(nullIndex + 1)..]);

            var commitLines = commitContent.Split('\n');

            var commitTreeLine = commitLines.Where(commit => commit.StartsWith("tree ")).FirstOrDefault();
            Assert.NotNull(commitTreeLine);

            var commitTreeParts = commitTreeLine.Split(" ", 2);
            Assert.NotEmpty(commitTreeParts);

            var commitTreeSha1 = commitTreeParts[1];

            var treeData = Utils.GetObjectDataBySha1(commitTreeSha1);
            var nullIndexHeader = Array.IndexOf(treeData, (byte)0);
            var treeContent = treeData.Skip(nullIndexHeader + 1).ToArray();

            int modeEnd = Array.IndexOf(treeContent, (byte)0x20, 0);
            int nameEnd = Array.IndexOf(treeContent, (byte)0, modeEnd + 1);

            var blobSha1 = treeContent.Skip(nameEnd + 1).Take(20).ToArray();

            Assert.Equal(sha1BlobExpected, Utils.Sha1BytesToString(blobSha1));
        }
    }
}
