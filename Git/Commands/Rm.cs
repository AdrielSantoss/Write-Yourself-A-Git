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

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                Console.WriteLine("Nenhum arquivo na staging area.");
                return;
            }

            var target = Path.GetRelativePath(Directory.GetCurrentDirectory(), args[0]);

            var indexLines = IndexUtils.GetIndexEntries();

            var removingEntries = indexLines.Keys.Where(index =>
                index == target ||
                index.StartsWith($"{target + Path.DirectorySeparatorChar}") 
            ).ToList();

            if (removingEntries.Count == 0)
            {
                Console.WriteLine($"'{target}' não está na staging area.");
                return;
            }

            var newContentLines = new List<string>();

            foreach (var file in indexLines.Keys)
            {
                if (removingEntries.Contains(file)) 
                {
                    continue;
                }

                newContentLines.Add($"{indexLines[file]} {file}");
            }

            IndexUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");

            if (removingEntries.Count == 1)
            {
                Console.WriteLine($"Arquivo '{removingEntries[0]}' removido da staging area.");
            }
            else
            {
                Console.WriteLine($"Removidos {removingEntries.Count} arquivos da staging area (diretório '{target}').");
            }
        }
    }
}
