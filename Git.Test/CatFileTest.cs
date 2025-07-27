using Csharp.Commands;
using Csharp.Core;
using Csharp.Test.Configs;
using System.Text;

namespace Csharp.Test
{
    public class CatFileTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void CatFile_DecompressAndReadBlobObject()
        {
            HashObjectTest.HashObject_CreateAndCompressBlobObject();

            var sha1Expected = Utils.GetSha1FromBlob("teste.txt");

            var Objectcontent = CatFile.Execute(["-p", sha1Expected]);
            var ObjectcontentExpected = File.ReadAllText("teste.txt");

            Assert.Equal(ObjectcontentExpected, Objectcontent);
        }
    }
}
