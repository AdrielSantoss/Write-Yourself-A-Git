using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Text;
namespace Git.Test
{
    public class AddTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void Add_HashBlobAndWriteIndex()
        {
            var fileName = "testeAdd.txt";
            var content = "test add";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            var sha1 = BlobUtils.GetSha1FromBlob(fileName);

            Add.Execute([fileName]);

            var lines = CommitUtils.GetIndexEntries();
            var parts = new List<string>();

            foreach (var line in lines)
            {
                parts.AddRange(line.Split(" ", 2));
            }

            Assert.NotEmpty(lines);
            Assert.NotEmpty(parts);
            Assert.Contains(fileName, parts);
            Assert.Contains(sha1, parts);
        }
    }
}
