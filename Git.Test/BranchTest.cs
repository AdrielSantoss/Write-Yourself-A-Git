using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
using System.Text;
namespace Git.Test
{
    public class BranchTest : IClassFixture<InitFixture>
    {
        [Theory]
        [InlineData("branchTest", "branchTest", "branchContent")]
        public static void Branch_CreateBranchFileAndVerifyContent(string branchName, string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            Add.Execute([fileName]);

            var indexEntries = IndexUtils.GetIndexEntries();
            Assert.NotEmpty(indexEntries);

            var indexEntry = indexEntries[fileName];
            Assert.NotNull(indexEntry);

            var sha1BlobExpected = BlobUtils.GetSha1FromBlob(fileName);
            Assert.Contains(sha1BlobExpected, indexEntries.Values);

            var commitSha1 = Commit.Execute(["-m", "commit test"]);
            Assert.NotNull(commitSha1);

            Branch.Execute([branchName]);

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var branchPath = Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}");
            Assert.True(Path.Exists(branchPath));

            var branchContent = File.ReadAllText(branchPath);
            Assert.Equal(commitSha1, branchContent);
        }

        [Theory]
        [InlineData("branchTest2")]
        public void Branch_CreateBranchAndDelete(string branchName)
        {
            Branch_CreateBranchFileAndVerifyContent(branchName, "branchTest2", "branchContent2");

            Branch.Execute(["-d", branchName]);

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var branchPath = Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}");
            Assert.False(Path.Exists(branchPath));
        }

        [Theory]
        [InlineData("branchTest3", "branchTest4", "branchTest5")]
        public void Branch_CreateBranchesAndListAllBranches(string branchName, string branchName2, string branchName3)
        {
            Branch_CreateBranchFileAndVerifyContent(branchName, "branchTest3", "branchContent3");
            Branch_CreateBranchFileAndVerifyContent(branchName2, "branchTest4", "branchContent4");
            Branch_CreateBranchFileAndVerifyContent(branchName3, "branchTest5", "branchContent5");

            var allBranches = Branch.Execute([]);

            Assert.NotNull(allBranches);
            Assert.Contains(branchName, allBranches);
            Assert.Contains(branchName2, allBranches);
            Assert.Contains(branchName3, allBranches);
        }
    }
}
