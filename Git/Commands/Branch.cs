using Git.Core;

namespace Git.Commands
{
    public class Branch
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run -- branch <nome_do_branch>");
                return;
            }

            var branchName = args[0];

            var existHeadFile = BranchUtils.GetCommitHeadFromBranch(branchName);

            if (existHeadFile != null)
            {
                Console.WriteLine($"Já existe um branch com o nome {branchName}");
                return;
            }

            var lastCommitSha1 = CommitUtils.GetLastCommitSha1FromHead();

            BranchUtils.CreateOrUpdateBranch($"refs/heads/{branchName}", lastCommitSha1);

            Console.WriteLine($"Branch {branchName} criado com sucesso");
        }
    }
}
