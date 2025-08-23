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

            var target = args[0];

            var headCommit = CommitUtils.GetLastCommitSha1FromHead();
            if (headCommit == null)
            {
                Console.WriteLine("Nenhum commit encontrado para restaurar.");
                return;
            }

            var headTreeSha1 = CommitUtils.GetCommitTreeSha1(headCommit);
            var headEntries = TreeUtils.GetTreeEntriesFromSha1("", headTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            if (target == ".")
            {
                foreach (var entry in headEntries)
                {
                    Sha1Utils.WriteFileAndDirectoriesFromSha1(entry.Key, entry.Value.Sha1);
                }

                Console.WriteLine("Workspace restaurada com sucesso a partir do HEAD.");
            }
            else if (Directory.Exists(target))
            {
                foreach (var entry in headEntries)
                {
                    if (entry.Key.StartsWith(target))
                    {
                        Sha1Utils.WriteFileAndDirectoriesFromSha1(entry.Key, entry.Value.Sha1);
                    }
                }

                Console.WriteLine($"Diretório '{target}' restaurado com sucesso.");
            }
            else if (headEntries.ContainsKey(target))
            {
                Sha1Utils.WriteFileAndDirectoriesFromSha1(target, headEntries[target].Sha1);
                Console.WriteLine($"Arquivo '{target}' restaurado com sucesso.");
            }
            else
            {
                Console.WriteLine($"O caminho '{target}' não existe no último commit.");
            }
        }
    }
}