using Csharp.Commands;
using Csharp.Core;
using System.IO;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            var headFileContent = Utils.GetHeadFileContent();
            Console.WriteLine(headFileContent.Replace(@"ref: refs\heads\", "On branch "));
            Console.WriteLine();

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

                var fileName = Path.GetRelativePath(Directory.GetCurrentDirectory(), file);

                var (sha1, fullBlob) = Utils.WriteBlob(file);

                var indexLines = Utils.GetIndexFileContentLines();
                var currentLineWithFile = indexLines.FirstOrDefault(line => line.Contains(fileName));

                if (currentLineWithFile == null)
                {
                    ConsoleWithColor($"Untracked: {fileName}", ConsoleColor.Red);
                }
                else
                {
                    var parts = currentLineWithFile.Split(' ', 2); 
                    var lineFileSha1 = parts[0];
                    var lineFileName = Path.GetFileName(parts[1]);

                    if (Path.GetFileName(fileName) == lineFileName)
                    {
                        if (sha1 == lineFileSha1)
                        {
                            ConsoleWithColor($"Staged: {parts[1]}", ConsoleColor.Green);
                        }
                        else
                        {
                            ConsoleWithColor($"Modified: {parts[1]}", ConsoleColor.Red);
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

        public static void ConsoleWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
