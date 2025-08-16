using Csharp.Commands;
using Git.Core;

namespace Git.Commands
{
    public class Reset
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run -- reset <arquivo>");
                return;
            }

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                Console.WriteLine("Nenhum arquivo na staging area.");
                return;
            }

            var file = args[0];

            var content = File.ReadAllText(pathIndex);
            var lines = string.IsNullOrWhiteSpace(content) ? Array.Empty<string>() : content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var newContentLines = new List<string>();
            var found = false;

            foreach (var line in lines)
            {
                var parts = line.Split(' ', 2);

                if (parts.Length != 2)
                    continue;

                var fileName = parts[1];

                if (fileName == file)
                {
                    found = true;
                    continue;
                }
                else
                {
                    newContentLines.Add(line);
                }
            }

            if (!found) 
            {
                Console.WriteLine($"Arquivo '{file}' não está na staging area.");
                return;
            }

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
            Console.WriteLine($"Arquivo '{file}' removido da staging area.");
        }
    }
}
