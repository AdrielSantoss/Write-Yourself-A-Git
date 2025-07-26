using Csharp.Commands;
using Csharp.Core;
using System.Text;

namespace Csharp.Test
{
    public class HashObjectTest : IClassFixture<GitAdrFixture>
    {
        [Fact]
        public void GitAdrInitAndHashObject()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            var fileName = "teste.txt";
            var content = "test hash-object";
            File.WriteAllText(fileName, content, Encoding.UTF8);

            HashObject.Execute(["-w", fileName]);

            var sha1Expected = Utils.GetSha1FromBlob(fileName);
            var pathObject = Path.Combine(gitAdrDir, "objects", sha1Expected.Substring(0, 2));

            Console.WriteLine(sha1Expected.Substring(2));

            Assert.True(File.Exists(Path.Combine(pathObject, sha1Expected.Substring(2))), "Objeto não encontrado.");
        }
    }
}
