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

            var target = NormalizePath(args[0]);
            var content = File.ReadAllText(pathIndex);
            var lines = string.IsNullOrWhiteSpace(content)
                ? Array.Empty<string>()
                : content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var newContentLines = new List<string>();
            var removedEntries = new List<string>();

            foreach (var line in lines)
            {
                var parts = line.Split(' ', 2);
                if (parts.Length != 2)
                {
                    continue;
                }

                var fileName = NormalizePath(parts[1]);

                if (fileName == target)
                {
                    removedEntries.Add(fileName);
                    continue;
                }

                if (Directory.Exists(target) && fileName.StartsWith(target + Path.DirectorySeparatorChar))
                {
                    removedEntries.Add(fileName);
                    continue;
                }

                newContentLines.Add(line);
            }

            if (removedEntries.Count == 0)
            {
                Console.WriteLine($"'{target}' não está na staging area.");
                return;
            }

            IndexUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");

            if (removedEntries.Count == 1)
            {
                Console.WriteLine($"Arquivo '{removedEntries[0]}' removido da staging area.");
            }
            else
            {
                Console.WriteLine($"Removidos {removedEntries.Count} arquivos da staging area (diretório '{target}').");
            }
        }

        private static string NormalizePath(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var repoRoot = Directory.GetParent(Path.Combine(Directory.GetCurrentDirectory(), ".gitadr"))!.FullName;
            var relativePath = Path.GetRelativePath(repoRoot, fullPath);

            return relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}
