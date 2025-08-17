using Git.Core;

namespace Git.Commands
{
    public class Switch
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run -- switch <nome_do_branch>");
                return;
            }

            var branchName = args[0];

            var existHeadFile = BranchUtils.GetCommitHeadFromBranch(branchName);

            if (existHeadFile == null)
            {
                Console.WriteLine($"Não existe um branch com o nome {branchName}");
                return;
            }

            BranchUtils.WriteHead(@$"ref: refs\heads\{branchName}");

            var commitHead = CommitUtils.GetLastCommitSha1FromHead();
            var treeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);

            var indexLines = new List<string>();
               
            var newIndexLines = RecursiveReadTree("", treeSha1, indexLines);

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newIndexLines) + "\n");

            Console.WriteLine($"Branch atual alterado com sucesso para {branchName}");
        }

        public static List<string> RecursiveReadTree(string prefix, string treeSha1, List<string> indexLines)
        {
            var treeEntries = TreeUtils.GetTreeEntriesFromSha1(treeSha1);

            foreach (var entry in treeEntries)
            {
                var fullPath = TreeUtils.CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    RecursiveReadTree(fullPath, entry.Sha1, indexLines);
                }
                else
                {
                    indexLines.Add($"{entry.Sha1} {fullPath}");
                }
            }

            return indexLines;
        }
    }
}
