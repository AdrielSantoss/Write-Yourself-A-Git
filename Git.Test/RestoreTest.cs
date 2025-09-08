using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Text;
namespace Git.Test
{
    public class RestoreTest : IClassFixture<InitFixture>
    {
        [Theory]
        [InlineData("restoreTest", "restoreTest 1")]
        public void Restore_RestoreChanges(string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Add.Execute([fileName]);

            var indexEntries = IndexUtils.GetIndexEntries();
            Assert.NotEmpty(indexEntries);

            var indexEntry = indexEntries[fileName];
            Assert.NotNull(indexEntry);

            var sha1BlobExpected = BlobUtils.GetSha1FromBlob(fileName);
            Assert.Contains(sha1BlobExpected, indexEntries.Values);

            var commitSha1 = Commit.Execute(["-m", "commit restore test"]);
            Assert.NotNull(commitSha1);

            File.WriteAllText(fileName, "new content", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            var newSha1BlobFile = BlobUtils.GetSha1FromBlob(fileName);
            Assert.NotEqual(newSha1BlobFile, sha1BlobExpected);

            Restore.Execute([fileName]);            

            newSha1BlobFile = BlobUtils.GetSha1FromBlob(fileName);
            Assert.Equal(newSha1BlobFile, sha1BlobExpected);
        }
    }
}
