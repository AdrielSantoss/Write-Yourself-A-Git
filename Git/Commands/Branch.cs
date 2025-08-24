using Git.Core;

namespace Git.Commands
{
    public class Branch
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
                var branchs = Directory.GetFiles(Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads"));
                var head = BranchUtils.GetHead().Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty);

                foreach (var branch in branchs)
                {
                    var name = Path.GetFileName(branch);
                    Console.WriteLine($"{(name == head ? "*" : string.Empty)} {name}");
                }

                return;
            }

            var branchName = args[0];

            if (args.Length == 2 && args[0] == "-d") 
            {
                branchName = args[1];
                var head = BranchUtils.GetHead().Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty);

                if (branchName == head)
                {
                    Console.WriteLine("Não é possivel excluir o HEAD.");

                    return;
                }

                var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
                File.Delete(Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}"));

                return;
            }

            var existHeadFile = BranchUtils.GetCommitHeadFromBranch(branchName);

            if (existHeadFile != null)
            {
                Console.WriteLine($"Já existe um branch com o nome {branchName}");
                return;
            }

            var lastCommitSha1 = CommitUtils.GetLastCommitSha1FromHead();

            BranchUtils.CreateOrUpdateBranch($"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}", lastCommitSha1);

            Console.WriteLine($"Branch {branchName} criado com sucesso");
        }
    }
}
