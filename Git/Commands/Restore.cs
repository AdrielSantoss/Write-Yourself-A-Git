using Git.Core;

namespace Git.Commands
{
    public class Restore
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr restore <arquivo | diretório | .>");
                return;
            }

            var headCommit = CommitUtils.GetLastCommitSha1FromHead();

            if (headCommit == null)
            {
                Console.WriteLine("Nenhum commit encontrado para restaurar.");
                return;
            }

            var commitTree = CommitUtils.GetCommitTreeSha1(headCommit);
            var commitEntries = TreeUtils.GetTreeEntriesFromSha1(string.Empty, commitTree, new Dictionary<string, (string Mode, string Sha1)>());

            var target = args[0];
            var removeIndex = args[0] == "--staged";

            if (removeIndex)
            {
                if (args.Length == 2)
                {
                    target = args[1];
                }
                else
                {
                    Console.WriteLine("Uso: gitadr restore <arquivo | diretório | .>");
                    return;
                }                
            }
                  
            if (target == ".")
            {
                ExecuteRecursive(Directory.GetCurrentDirectory(), commitEntries, removeIndex);
                Console.WriteLine("Workspace restaurada com sucesso a partir do HEAD.");

                return;
            }

            target = Path.GetRelativePath(Directory.GetCurrentDirectory(), target);

            if (Directory.Exists(target))
            {
                ExecuteRecursive(target, commitEntries, removeIndex);
                Console.WriteLine($"Diretório '{target}' restaurado com sucesso.");

                return;
            }

            if (File.Exists(target) && commitEntries.ContainsKey(target))
            {
                RestoreFile(target, commitEntries[target].Sha1, removeIndex);
                Console.WriteLine($"Arquivo '{target}' restaurado com sucesso.");

                return;
            }

            Console.WriteLine($"O caminho '{target}' não existe no último commit.");
        }

        public static void ExecuteRecursive(string directory, Dictionary<string, (string Mode, string Sha1)> commitEntries, bool removeIndex)
        {
            foreach (var wsFile in Directory.GetFiles(directory))
            {
                var file = Path.GetRelativePath(Directory.GetCurrentDirectory(), wsFile);
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(file) == ignoreFile))
                {
                    continue;
                }

                var indexFileSha1 = commitEntries.ContainsKey(file) ? commitEntries[file].Sha1 : null;

                if (indexFileSha1 != null) 
                {
                    RestoreFile(file, indexFileSha1, removeIndex);
                }
            }

            foreach (var wsDirectory in Directory.GetDirectories(directory))
            {
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(wsDirectory) == ignoreFile))
                {
                    continue;
                }

                ExecuteRecursive(Path.Combine(directory, wsDirectory), commitEntries, removeIndex);
            }
        }

        private static void RestoreFile(string file, string sha1, bool romoveIndex)
        {           
            if (romoveIndex)
            {
                Rm.Execute([file]);
                Add.AddOrUpdateIndexFile(file, sha1);

                return;
            }

            Sha1Utils.WriteFileAndDirectoriesFromSha1(file, sha1);
        }
    }
}