
using Csharp.Commands;
using Csharp.Core;
using Git.Core;
using System;
using System.Text;

namespace Git.Commands
{
    public class Commit
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-m")
            {
                Console.WriteLine("Uso: dotnet run -- commit [-m] <mensagem_do_commit>");
                return string.Empty;
            }

            var lines = Utils.GetIndexFileContentLines();

            if (!lines.Any())
            {
                throw new Exception("Nenhum arquivo na staging area.");
            }

            var rootSha1 = "";

            foreach (var line in lines)
            {
                var parts = line.Split(' ', 2);

                if (parts.Length != 2)
                    continue;

                var fileSha1 = parts[0];
                var path = parts[1];

                var pathItems = path.TrimStart('/').Split('/').ToArray();
                var file = pathItems.Last();

                var currentSha1 = fileSha1;
                var currentName = file;
                var currentMode = "100644";

                if (pathItems.Length > 1)
                {
                    pathItems = pathItems.Take(pathItems.Length - 1).ToArray();

                    foreach (var item in pathItems.Reverse())
                    {
                        currentSha1 = TreeObject.WriteTree(
                        new List<TreeEntry>() {
                        new TreeEntry()
                            {
                                Mode = currentMode,
                                Name = currentName,
                                Sha1 = currentSha1
                            }
                            }
                        );

                        currentName = item;
                        currentMode = "040000";
                    }
                }

                rootSha1 = TreeObject.WriteTree(
                    new List<TreeEntry>() {
                        new TreeEntry()
                        {
                            Mode = currentMode,
                            Name = currentName,
                            Sha1 = currentSha1
                        }
                    }
                );
            }

            var commitSha1 = CommitObject.WriteCommit(rootSha1, args[1]);

            UpdateHead(commitSha1);

            Utils.WriteIndexFile(string.Empty);

            Console.WriteLine(commitSha1);

            return commitSha1;
        }

        private static void UpdateHead(string commitSha1)
        {
            var refs = Utils.GetHeadFileContent();
            var parts = refs.Split(" ", 2);

            Utils.WriteBranchFile(parts[1], commitSha1);
        }
    }
}
