using Git.Core;

namespace Git.Commands
{
    public class Rm
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr rm <arquivo | diretório>");
                return;
            }

            var indexLines = IndexUtils.GetIndexEntries(); 

            if (indexLines.Count == 0)
            {
                Console.WriteLine("Nenhum arquivo na staging area.");
                return;
            }

            var target = Path.GetRelativePath(Directory.GetCurrentDirectory(), args[0]);

            if (!indexLines.ContainsKey(target) && !indexLines.Any(index => index.Key.StartsWith(target + Path.DirectorySeparatorChar)))
            {
                Console.WriteLine($"'{target}' não encontrado na staging area.");

                return;
            }

            if (Directory.Exists(target))
            {
                ExecuteRecursive(target, indexLines);

                return;
            }

            if (File.Exists(target))
            {
                RemoveIndexFile(target, indexLines);

                return;
            }
        }

        private static void ExecuteRecursive(string directory, Dictionary<string, string> indexLines)
        {
            foreach (var wsFile in Directory.GetFiles(directory))
            {
                var file = Path.GetRelativePath(Directory.GetCurrentDirectory(), wsFile);
                var indexFileSha1 = indexLines.ContainsKey(file) ? indexLines[file] : null;

                if (indexFileSha1 != null)
                {
                    RemoveIndexFile(file, indexLines);
                }
            }

            foreach (var wsDirectory in Directory.GetDirectories(directory))
            {
                ExecuteRecursive(wsDirectory, indexLines);
            }
        }

        private static void RemoveIndexFile(string file, Dictionary<string, string> indexLines)
        {
            var newContentLines = new List<string>();

            foreach (var line in indexLines.Keys)
            {
                if (line == file)
                {
                    Console.WriteLine($"Arquivo '{line}' removido da staging area.");
                    continue;
                }

                newContentLines.Add($"{indexLines[line]} {line}");
            }

            IndexUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }
    }
}
