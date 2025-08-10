using Csharp.Commands;
using Csharp.Core;
using System.Text;

namespace Git.Commands
{
    public class Add
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run -- add <arquivo>");
                return;
            }

            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                File.WriteAllText(pathIndex, string.Empty);
            }

            var file = args[0];

            var content= File.ReadAllText(pathIndex);
            var sha1 = HashObject.Execute(["-w", file]);
            
            if (content.Contains(sha1))
            {
                Console.WriteLine($"{file} já esta inserido na stage area.");
                return;
            }
            
            File.AppendAllText(pathIndex, $"{sha1} {file}\n");
        }
    }
}
