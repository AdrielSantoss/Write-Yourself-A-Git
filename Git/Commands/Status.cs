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

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
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
                    Console.WriteLine($"Arquivo novo: {file}");
                }
                else
                {
                    var parts = currentLineWithFile.Split(' ', 2);
                    var lineFileSha1 = parts[0];
                    var lineFileName = parts[1];

                    if (fileName == lineFileName)
                    {
                        if (sha1 == lineFileSha1)
                        {
                            Console.WriteLine($"Arquivo na staging area: {fileName}");
                        }
                        else
                        {
                            Console.WriteLine($"Arquivo alterado: {fileName}");
                        }
                    }
                }
            }
        }
    }
}
