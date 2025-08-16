using Csharp.Commands;
using Csharp.Test.Configs;
using Git.Core;
using System.Text;

namespace Csharp.Test
{
    public class HashObjectTest : IClassFixture<InitFixture>
    {
        [Fact]
        public static void HashObject_CreateAndCompressBlobObject()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            var fileName = "teste.txt";
            var content = "test hash-object";
            File.WriteAllText(fileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            HashObject.Execute(["-w", fileName]);

            var sha1Expected = BlobUtils.GetSha1FromBlob(fileName);
            var pathObject = Path.Combine(gitAdrDir, "objects", sha1Expected.Substring(0, 2));

            Assert.True(File.Exists(Path.Combine(pathObject, sha1Expected.Substring(2))), "Objeto não encontrado.");
        }
    }
}
