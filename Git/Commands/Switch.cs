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

            var workSpaceEntries  = CommitUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory());
            var indexEntries = CommitUtils.GetIndexEntries(false);

            var targetBranchHead = BranchUtils.GetCommitHeadFromBranch(branchName);
            var targetBranchTreeSha1 = CommitUtils.GetCommitTreeSha1(targetBranchHead!);
            var targetBranchEntries = TreeUtils.GetTreeEntriesFromSha1("", targetBranchTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var commitHead = CommitUtils.GetLastCommitSha1FromHead();
            var commitHeadTreeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);
            var headEntries = TreeUtils.GetTreeEntriesFromSha1("", commitHeadTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var fullEntries = workSpaceEntries.Keys.Union(indexEntries.Keys);
            
            foreach (var entry in fullEntries)
            {
                var wsSha1 = workSpaceEntries.ContainsKey(entry) ? workSpaceEntries[entry] : null;
                var idxSha1 = indexEntries.ContainsKey(entry) ? indexEntries[entry] : null;
                var tgtSha1 = targetBranchEntries.ContainsKey(entry) ? targetBranchEntries[entry].Sha1 : null;
                var headSha1 = headEntries.FirstOrDefault(e => e.Key == entry).Value.Sha1;

                if (idxSha1 == null && wsSha1 != null && tgtSha1 != null)
                {
                    Console.WriteLine($"Arquivo não rastreado '{entry}' seria sobrescrito ao trocar de branch.");
                    return;
                }

                if (wsSha1 != null && idxSha1 != null && wsSha1 != idxSha1)
                {
                    Console.WriteLine($"Existem mudanças não commitadas na workspace para '{entry}'.");
                    return;
                }

                if (tgtSha1 != null && idxSha1 != null && idxSha1 != tgtSha1 && idxSha1 != headSha1)
                {
                    Console.WriteLine($"Existem mudanças staged que conflitam com o branch alvo para '{entry}'.");
                    return;
                }
            }

            var indexLines = new List<string>();
               
            CommitUtils.RecursiveUpdateIndexFromTree("", targetBranchTreeSha1, indexLines);

            var fullNewWs = targetBranchEntries.Keys.Union(workSpaceEntries.Keys);

            foreach (var entry in fullNewWs)
            {
                var tgtSha1 = targetBranchEntries.ContainsKey(entry) ? targetBranchEntries[entry].Sha1 : null;
                var wsSha1 = workSpaceEntries.ContainsKey(entry) ? workSpaceEntries[entry] : null;

                if (tgtSha1 != null && wsSha1 != null)
                {
                    if (tgtSha1 != wsSha1)
                    {
                        Sha1Utils.WriteFileAndDirectoriesFromSha1(entry, tgtSha1);
                    }

                    continue;
                }

                if (tgtSha1 == null && wsSha1 != null)
                {
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), entry));
                    continue;
                }

                if (wsSha1 == null && tgtSha1 != null)
                {
                    Sha1Utils.WriteFileAndDirectoriesFromSha1(entry, tgtSha1);
                    continue;
                }
            }

            BranchUtils.WriteHead(@$"ref: refs\heads\{branchName}");

            Console.WriteLine($"Branch atual alterado com sucesso para {branchName}");
        }
    }
}
