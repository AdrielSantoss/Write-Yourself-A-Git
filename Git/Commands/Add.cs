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

            var file = args[0];

            var sha1 = HashObject.Execute(new string[] { "-w", file });

            var lines = Utils.GetIndexFileContentLines(true);

            var newContentLines = new List<string>();
            var found = false;

            foreach (var line in lines)
            {
                var parts = line.Split(' ', 2);

                if (parts.Length != 2)
                    continue;

                var fileSha1 = parts[0];
                var fileName = parts[1];

                if (fileName == file)
                {
                    found = true;
                    if (fileSha1 == sha1)
                    {
                        Console.WriteLine($"{file} já está inserido na staging area.");
                        return;
                    }
                    else
                    {
                        newContentLines.Add($"{sha1} {fileName}");
                    }
                }
                else
                {
                    newContentLines.Add(line);
                }
            }

            if (!found)
            {
                newContentLines.Add($"{sha1} {file}");
            }

            Utils.WriteIndexFile(string.Join('\n', newContentLines) + "\n");
        }
    }
}
