using Csharp.Core;
using Csharp.Test.Configs;
using Git.Commands;
using System.Text;
namespace Git.Test
{
    public class ResetTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void Reset_FindBlobAndRemoveIndexLine()
        {
            var fileName = "testeReset.txt";
            var content = "test reset";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            var sha1 = Utils.GetSha1FromBlob(fileName);

            Add.Execute([fileName]);
            var lines = Utils.GetIndexFileContentLines();
            Assert.NotEmpty(lines);

            Reset.Execute([fileName]);
            lines = Utils.GetIndexFileContentLines();
            Assert.Empty(lines);
        }
    }
}
