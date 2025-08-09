using Csharp.Commands;
using Csharp.Test;
using Git.Commands;

namespace Git.Test
{
    public class LsTreeTest
    {
        [Fact]
        public void LsTree_WriteTreeAndCompareTreeContent()
        {
            var (workSpace, treeSha1) = WriteTreeTest.CreateWorksSpace_AndWriteTree();

            var lsTreeContent = LsTree.Execute(["-p", treeSha1]);

            Assert.Equal(
                "100644 blob 27f041740e69694eabbff7d2af994397778a8c51 teste.txt040000 tree a0fdb95f4eda2142cafbc0829b9e987239a43dcf src", 
                lsTreeContent
            );
        }
    }
}
