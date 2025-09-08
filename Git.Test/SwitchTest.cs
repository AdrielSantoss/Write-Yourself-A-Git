using Csharp.Test.Configs;
using Git.Commands;
using Git.Core;
namespace Git.Test
{
    public class SwitchTest : IClassFixture<InitFixture>
    {
        [Theory]
        [InlineData("branchTest6")]
        public void Switch_CreateBranchAndSwitchHead(string branchName)
        {
            BranchTest.Branch_CreateBranchFileAndVerifyContent(branchName, "branchTest3", "branchContent3");

            Switch.Execute([branchName]);

            var head = BranchUtils.GetHead();
            Assert.Equal($"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}", head);

            Switch.Execute(["master"]);
        }
    }
}
