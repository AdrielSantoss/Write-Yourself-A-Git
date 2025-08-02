using Csharp.Commands;
using Csharp.Core;
using Csharp.Test.Configs;
using System.Text;

namespace Csharp.Test
{
    public class WriteTreeTest : IClassFixture<InitFixture>
    {
        [Fact]
        public void WriteTree_CreateBlobsAndTreeObjects()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var gitAdrDir = Path.Combine(currentDirectory, ".gitadr");

            var fileName = "teste.txt";
            var content = "test write-tree";

            var diretorio1 = Path.Combine(currentDirectory, "diretorio1");
            var diretorio2 = Path.Combine(currentDirectory, "diretorio2");
            var diretorio3 = Path.Combine(currentDirectory, "diretorio3");

            Directory.CreateDirectory(diretorio1);
            Directory.CreateDirectory(diretorio2);
            Directory.CreateDirectory(diretorio3);

            File.WriteAllText(Path.Combine(diretorio1, fileName), content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.WriteAllText(Path.Combine(diretorio2, fileName), content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.WriteAllText(Path.Combine(diretorio3, fileName), content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            WriteTree.Execute();

         //Assert.Equal(ObjectcontentExpected, Objectcontent);
        }
    }
}
