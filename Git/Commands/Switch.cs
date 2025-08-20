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
            var targetBranchEntries = new Dictionary<string, string>();
            targetBranchEntries = RecursiveReadTree2("", targetBranchTreeSha1, targetBranchEntries);

            var fullEntries = workSpaceEntries.Keys.Union(indexEntries.Keys);

            foreach (var entry in fullEntries)
            {
                var wsSha1 = workSpaceEntries.ContainsKey(entry) ? workSpaceEntries[entry] : null;
                var idxSha1 = indexEntries.ContainsKey(entry) ? indexEntries[entry] : null;
                var tgtSha1 = targetBranchEntries.ContainsKey(entry) ? targetBranchEntries[entry] : null;

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

                if (tgtSha1 != null && idxSha1 != tgtSha1)
                {
                    Console.WriteLine($"Existem mudanças staged que conflitam com o branch alvo para '{entry}'.");
                    return;
                }
            } 

            BranchUtils.WriteHead(@$"ref: refs\heads\{branchName}");

            var indexLines = new List<string>();
               
            var newIndexLines = RecursiveReadTree("", targetBranchTreeSha1, indexLines);

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newIndexLines) + "\n");

            var fullNewWs = targetBranchEntries.Keys.Union(workSpaceEntries.Keys);

            foreach (var entry in fullNewWs)
            {
                var tgtSha1 = targetBranchEntries.ContainsKey(entry) ? targetBranchEntries[entry] : null;
                var wsSha1 = workSpaceEntries.ContainsKey(entry) ? workSpaceEntries[entry] : null;

                if (tgtSha1 != null && wsSha1 != null)
                {
                    if (tgtSha1 != wsSha1)
                    {
                        WriteFileFromSha1(entry, tgtSha1);
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
                    WriteFileFromSha1(entry, tgtSha1);
                    continue;
                }
            }

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

        private static Dictionary<string, string> RecursiveReadTree2(string prefix, string treeSha1, Dictionary<string, string> dict)
        {
            var entries = TreeUtils.GetTreeEntriesFromSha1(treeSha1);

            foreach (var entry in entries)
            {
                var fullPath = TreeUtils.CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    RecursiveReadTree2(fullPath, entry.Sha1, dict);
                }
                else
                {
                    dict[fullPath] = entry.Sha1;
                }
            }

            return dict;
        }

        public static void WriteFileFromSha1(string path, string sha1)
        {
            var data = Sha1Utils.GetObjectDataBySha1(sha1);
            var nullIndex = Array.IndexOf(data, (byte)0);
            var blob = data[(nullIndex + 1)..];

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllBytes(path, blob);
        }
    }
}
