using Csharp.Commands;
using Csharp.Core;
using System.IO;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            Console.WriteLine(Utils.GetHeadFileContent());

            ExecuteRecursive(Directory.GetCurrentDirectory());
        }

        public static void ExecuteRecursive(string directory)
        {

            foreach (var file in Directory.GetFiles(directory))
            {
                if (WriteTree.ignoreFiles.Any(ignore => file.Contains(ignore)))
                {
                    continue;
                }

                var fileName = Path.GetFileName(file);

                var (sha1, fullBlob) = Utils.WriteBlob(file);

                var indexLines = Utils.GetIndexFileContentLines();
                var currentLineWithFile = indexLines.FirstOrDefault(line => line.Contains(fileName));

                if (currentLineWithFile == null)
                {
                    Console.WriteLine($"Arquivo novo: {fileName}");
                }
                else
                {
                    var parts = currentLineWithFile.Split(' ', 2);
                    var lineFileSha1 = parts[0];
                    var lineFileName = Path.GetFileName(parts[1]);

                    if (fileName == lineFileName)
                    {
                        if (sha1 == lineFileSha1)
                        {
                            Console.WriteLine($"Arquivo na staging area: {parts[1]}");
                        }
                        else
                        {
                            Console.WriteLine($"Arquivo alterado: {fileName}");
                        }
                    }
                }
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
    }
}
