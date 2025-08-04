using Csharp.Commands;
using Csharp.Core;
using Csharp.Test.Configs;
using System.Text;

namespace Csharp.Test
{
    public class WriteTreeTest
    {
        [Fact]
        public void WriteTree_CreateBlobsAndTreeObjects()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
            Directory.SetCurrentDirectory(tempRoot);

            Init.Execute();

            var fileName = "teste.txt";
            var content = "test write-tree";

            var dir = Path.Combine(tempRoot, "src");
            Directory.CreateDirectory(dir);

            var filePath = Path.Combine(dir, fileName);
            File.WriteAllText(filePath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            var contentFileBytes = File.ReadAllBytes(filePath);
            var blobHeader = $"blob {contentFileBytes.Length}\0";
            var fullBlob = Utils.CombineBytes(Encoding.UTF8.GetBytes(blobHeader), contentFileBytes);
            var blobSha1 = Utils.ComputeSha1(fullBlob);
            ObjectStore.WriteObject(blobSha1, fullBlob);

            var treeEntriesSrc = new List<TreeEntry>
            {
                new TreeEntry
                {
                    Mode = "100644",
                    Name = fileName,
                    Sha1 = blobSha1
                }
            };
            var srcTreeSha1 = TreeObject.WriteTree(treeEntriesSrc);

            var treeEntriesRoot = new List<TreeEntry>
            {
                new TreeEntry
                {
                    Mode = "040000",
                    Name = "src",
                    Sha1 = srcTreeSha1
                }
            };

            var expectedRootSha1 = TreeObject.WriteTree(treeEntriesRoot);
            var actualRootSha1 = WriteTree.WriteTreeRecursive(tempRoot);

            Assert.Equal(expectedRootSha1, actualRootSha1);
        }
    }
}
