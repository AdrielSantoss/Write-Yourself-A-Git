using Csharp.Core;
using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Text;

namespace Git.Test
{
    public class RmTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void Reset_FindBlobAndRemoveIndexLine()
        {
            var fileName = "testeRm.txt";
            var content = "test rm";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            var sha1 = BlobUtils.GetSha1FromBlob(fileName);

            Add.Execute([fileName]);
            var lines = CommitUtils.GetIndexEntries();
            Assert.NotEmpty(lines);

            Rm.Execute([fileName]);
            lines = CommitUtils.GetIndexEntries();
            Assert.Empty(lines);
        }
    }
}
