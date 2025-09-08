using Git.Core;

namespace Git.Commands
{
    public class Branch
    {
        public static List<string>? Execute(string[] args)
        {
            if (args.Length < 1)
            {
                var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
                var branchs = Directory.GetFiles(Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads"));
                var head = BranchUtils.GetHead().Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty);
                var allBranches = new List<string>();

                foreach (var branch in branchs)
                {
                    var name = Path.GetFileName(branch);
                    var formattedName = $"{(name == head ? "*" : string.Empty)} {name}";

                    Console.WriteLine(formattedName);
                    allBranches.Add(formattedName.Trim());
                }

                return allBranches;
            }

            var branchName = args[0];

            if (args.Length == 2 && args[0] == "-d") 
            {
                branchName = args[1];
                var head = BranchUtils.GetHead().Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty);

                if (branchName == head)
                {
                    Console.WriteLine("Não é possivel excluir o HEAD.");

                    return null;
                }

                var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
                File.Delete(Path.Combine(gitDir, $"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}"));

                return null;
            }

            var existHeadFile = BranchUtils.GetCommitHeadFromBranch(branchName);

            if (existHeadFile != null)
            {
                Console.WriteLine($"Já existe um branch com o nome {branchName}");
                return null;
            }

            var lastCommitSha1 = CommitUtils.GetLastCommitSha1FromHead();

            if (string.IsNullOrWhiteSpace(lastCommitSha1))
            {
                Console.WriteLine($"Não é possivel criar um branch quando não existem commits.");
                return null;
            }

            BranchUtils.CreateOrUpdateBranch($"refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}{branchName}", lastCommitSha1);

            Console.WriteLine($"Branch {branchName} criado com sucesso");

            return null;
        }
    }
}
