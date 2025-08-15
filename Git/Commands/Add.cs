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

            var fileOrDirectory = args[0];

            if (!File.Exists(fileOrDirectory) && !Directory.Exists(fileOrDirectory))
            {
                Console.WriteLine("Arquivo ou diretório não encontrado.");
                return;
            }

            if (!File.Exists(fileOrDirectory) && Directory.Exists(fileOrDirectory)) 
            {
                AddOrUpdateIndexFile(Directory.GetFiles(fileOrDirectory).ToList());
                return;
            }

            if (File.Exists(fileOrDirectory) && !Directory.Exists(fileOrDirectory))
            {
                AddOrUpdateIndexFile(
                    new List<string>()
                    {
                        fileOrDirectory
                    }
                );
            }
        }

        public static void AddOrUpdateIndexFile(List<string> files)
        {
            foreach (var file in files)
            {
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
}
