using Git.Core;

namespace Git.Commands
{
    public class Restore
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr restore [--staged] <arquivo | diretório | .>");
                return;
            }

            bool restoreStaged = args[0] == "--staged";
            string target;

            if (restoreStaged)
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Uso: gitadr restore --staged <arquivo | diretório | .>");
                    return;
                }
                target = args[1];
            }
            else
            {
                target = args[0];
            }

            var headCommit = CommitUtils.GetLastCommitSha1FromHead();
            if (string.IsNullOrEmpty(headCommit))
            {
                Console.WriteLine("Nenhum commit encontrado. Não é possível restaurar do HEAD.");
                return;
            }

            var commitTree = CommitUtils.GetCommitTreeSha1(headCommit);
            var commitEntries = TreeUtils.GetTreeEntriesFromSha1(
                string.Empty,
                commitTree,
                new Dictionary<string, (string Mode, string Sha1)>()
            );

            target = Path.GetRelativePath(Directory.GetCurrentDirectory(), target);

            if (target == ".")
            {
                ExecuteRecursive(Directory.GetCurrentDirectory(), commitEntries, restoreStaged);
                Console.WriteLine(restoreStaged ? "Index restaurado a partir do HEAD." : "Workspace restaurada com sucesso a partir do HEAD.");
                return;
            }

            if (Directory.Exists(target))
            {
                ExecuteRecursive(target, commitEntries, restoreStaged);
                Console.WriteLine(restoreStaged ? $"Index do diretório '{target}' restaurado com sucesso." : $"Diretório '{target}' restaurado com sucesso.");
                return;
            }

            if (!commitEntries.ContainsKey(target))
            {
                Console.WriteLine($"O caminho '{target}' não existe no último commit. Não é possível restaurar.");
                return;
            }

            RestoreFile(target, commitEntries[target].Sha1, restoreStaged);
            Console.WriteLine(restoreStaged ? $"Index do arquivo '{target}' restaurado com sucesso." : $"Arquivo '{target}' restaurado com sucesso.");
        }

        public static void ExecuteRecursive(string directory, Dictionary<string, (string Mode, string Sha1)> commitEntries, bool restoreStaged)
        {
            foreach (var wsFile in Directory.GetFiles(directory))
            {
                var file = Path.GetRelativePath(Directory.GetCurrentDirectory(), wsFile);
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(file) == ignoreFile))
                {
                    continue;
                }

                if (commitEntries.TryGetValue(file, out var entry))
                {
                    RestoreFile(file, entry.Sha1, restoreStaged);
                }
            }

            foreach (var wsDirectory in Directory.GetDirectories(directory))
            {
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(wsDirectory) == ignoreFile))
                {
                    continue;
                }
                    
                ExecuteRecursive(wsDirectory, commitEntries, restoreStaged);
            }
        }

        private static void RestoreFile(string file, string sha1, bool restoreStaged)
        {
            if (restoreStaged)
            {
                Add.AddOrUpdateIndexFile(file, sha1);
            }
            else
            {
                Sha1Utils.WriteFileAndDirectoriesFromSha1(file, sha1);
            }
        }
    }
}
