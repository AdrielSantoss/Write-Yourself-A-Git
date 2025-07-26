using Csharp.Commands;
using Xunit.Sdk;

namespace Csharp.Test
{
    public class InitTst
    {
        [Fact]
        public void GitAdrInit_CreateGitAdrDirectory()
        {
            var gitAdrInit = new Init();

            gitAdrInit.Execute();

            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            Assert.True(
                Directory.Exists(gitAdrDir) &&
                Directory.Exists(Path.Combine(gitAdrDir, "objects")) &&
                Directory.Exists(Path.Combine(gitAdrDir, "refs")) &&
                File.Exists(Path.Combine(gitAdrDir, "HEAD"))
            );

            Directory.Delete(gitAdrDir, true );
        }
    }
}
