using Csharp.Commands;
using Csharp.Test.Configs;
using Git.Core;

namespace Csharp.Test
{
    public class CatFileTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void CatFile_DecompressAndReadBlobObject()
        {
            HashObjectTest.HashObject_CreateAndCompressBlobObject();

            var sha1Expected = BlobUtils.GetSha1FromBlob("teste.txt");

            var Objectcontent = CatFile.Execute(["-p", sha1Expected]);
            var ObjectcontentExpected = File.ReadAllText("teste.txt");

            Assert.Equal(ObjectcontentExpected, Objectcontent);
        }
    }
}
