using Csharp.Commands;

namespace Csharp.Test.Configs
{
    public class GitAdrFixture
    {
        public GitAdrFixture()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            if (Directory.Exists(gitAdrDir))
            {
                Directory.Delete(gitAdrDir, true);
            }

            Init.Execute();

            Assert.True(
                Directory.Exists(gitAdrDir),
                ".gitadr não foi criado.");

            Assert.True(
                Directory.Exists(Path.Combine(gitAdrDir, "objects")),
                "Diretório 'objects' não encontrado.");

            Assert.True(
                Directory.Exists(Path.Combine(gitAdrDir, "refs")),
                "Diretório 'refs' não encontrado.");

            Assert.True(
                File.Exists(Path.Combine(gitAdrDir, "HEAD")),
                "Arquivo 'HEAD' não encontrado.");
        }
    }
}
