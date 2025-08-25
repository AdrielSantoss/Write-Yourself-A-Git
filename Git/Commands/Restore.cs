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

            if (CommitUtils.GetLastCommitSha1FromHead() == null)
            {
                Console.WriteLine("Nenhum commit encontrado para restaurar.");
                return;
            }

            var target = args[0];
            var indexLines = IndexUtils.GetIndexEntries();

            if (target == ".")
            {
                ExecuteRecursive(Directory.GetCurrentDirectory(), indexLines);
                Console.WriteLine("Workspace restaurada com sucesso a partir do HEAD.");

                return;
            }

            target = Path.GetRelativePath(Directory.GetCurrentDirectory(), target);

            if (Directory.Exists(target))
            {
                ExecuteRecursive(target, indexLines);
                Console.WriteLine($"Diretório '{target}' restaurado com sucesso.");

                return;
            }

            if (File.Exists(target) && indexLines.ContainsKey(target))
            {
                Sha1Utils.WriteFileAndDirectoriesFromSha1(target, indexLines[target]);
                Console.WriteLine($"Arquivo '{target}' restaurado com sucesso.");

                return;
            }   
            
            Console.WriteLine($"O caminho '{target}' não existe no último commit.");
        }

        public static void ExecuteRecursive(string directory, Dictionary<string, string> indexLines)
        {
            foreach (var wsFile in Directory.GetFiles(directory))
            {
                var file = Path.GetRelativePath(Directory.GetCurrentDirectory(), wsFile);
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(file) == ignoreFile))
                {
                    continue;
                }

                var indexFileSha1 = indexLines.ContainsKey(file) ? indexLines[file] : null;

                if (indexFileSha1 != null) 
                {
                    Sha1Utils.WriteFileAndDirectoriesFromSha1(file, indexFileSha1);
                }
            }

            foreach (var wsDirectory in Directory.GetDirectories(directory))
            {
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(wsDirectory) == ignoreFile))
                {
                    continue;
                }

                ExecuteRecursive(Path.Combine(directory, wsDirectory), indexLines);
            }
        }
    }
}