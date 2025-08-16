using Csharp.Commands;
using Git.Core;

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

            if (fileOrDirectory == ".")
            {
                ExecuteRecursive(Directory.GetCurrentDirectory());
                return;
            }

            if (!File.Exists(fileOrDirectory) && !Directory.Exists(fileOrDirectory))
            {
                Console.WriteLine("Arquivo ou diretório não encontrado.");
                return;
            }

            if (!File.Exists(fileOrDirectory) && Directory.Exists(fileOrDirectory)) 
            {
                ExecuteRecursive(fileOrDirectory);
                return;
            }

            if (File.Exists(fileOrDirectory) && !Directory.Exists(fileOrDirectory))
            {
                AddOrUpdateIndexFile(fileOrDirectory);
                return;
            }
        }

        public static void ExecuteRecursive(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (WriteTree.ignoreFiles.Any(ignore => file.Contains(ignore)))
                {
                    continue;
                }

                AddOrUpdateIndexFile(file);
            }

            foreach (var subdir in Directory.GetDirectories(directory))
            {
                if (WriteTree.ignoreFiles.Any(ignore => subdir.Contains(ignore)))
                {
                    continue;
                }

                ExecuteRecursive(subdir);
            }
        }

        public static void AddOrUpdateIndexFile(string file)
        {
            var sha1 = HashObject.Execute(new string[] { "-w", file });

            var lines = CommitUtils.GetIndexEntries(true);

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

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }
    }
}
