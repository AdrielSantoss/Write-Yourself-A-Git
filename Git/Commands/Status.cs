using Csharp.Commands;
using Git.Core;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            var headFileContent = BranchUtils.GetHead();
            Console.WriteLine(headFileContent.Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", "On branch ") + "\n");

            ExecuteRecursive(Directory.GetCurrentDirectory());
        }

        public static void ExecuteRecursive(string directory)
        {
            var worksSpaceFiles = new Dictionary<string, string>();
            RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), worksSpaceFiles);

            var commitHead = CommitUtils.GetLastCommitSha1FromHead();
            var headFiles = new Dictionary<string, (string Mode, string Sha1)>();

            if (!string.IsNullOrWhiteSpace(commitHead))
            {
                var commitTreeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);
                RecursiveReadTree("", commitTreeSha1, headFiles);
            }

            var indexFiles = CommitUtils.GetIndexEntries(false);

            var staged = new List<string>();
            var modified = new List<string>();
            var deleted = new List<string>();
            var untracked = new List<string>();

            foreach (var file in indexFiles.Keys)
            {
                var shaIndex = indexFiles[file];
                if (headFiles.ContainsKey(file))
                {
                    if (headFiles[file].Sha1 != shaIndex)
                    {
                        staged.Add($"modified: {file}");
                    }
                }
                else
                {
                    staged.Add($"new file: {file}");
                }
            }

            foreach (var file in indexFiles.Keys)
            {
                var shaIndex = indexFiles[file];
                if (!worksSpaceFiles.ContainsKey(file))
                {
                    deleted.Add(file); 
                }
                else
                {
                    var sha1Ws = worksSpaceFiles[file];
                    if (sha1Ws != shaIndex)
                    {
                        modified.Add(file);
                    }
                }
            }

            foreach (var file in worksSpaceFiles.Keys)
            {
                if (!indexFiles.ContainsKey(file))
                {
                    untracked.Add(file);
                }
            }

            if (staged.Any())
            {
                Console.WriteLine("Changes to be committed:");
                foreach (var s in staged)
                {
                    ConsoleWithColor($"  {s}", ConsoleColor.Green);
                }

                Console.WriteLine();
            }

            if (modified.Any() || deleted.Any())
            {
                Console.WriteLine("Changes not staged for commit:");
                foreach (var m in modified)
                {
                    ConsoleWithColor($"  modified: {m}", ConsoleColor.Red);
                }
                    
                foreach (var d in deleted)
                {
                    ConsoleWithColor($"  deleted:  {d}", ConsoleColor.Red);
                }
                    
                Console.WriteLine();
            }

            if (untracked.Any())
            {
                Console.WriteLine("Untracked files:");
                foreach (var u in untracked)
                {
                    ConsoleWithColor($"  {u}", ConsoleColor.Red);
                }
                    
                Console.WriteLine();
            }

            if (!staged.Any() && !modified.Any() && !deleted.Any() && !untracked.Any())
            {
                Console.WriteLine("Nada para commitar, working tree limpa.");
            }
        }

        private static void RecursiveReadTree(string prefix, string treeSha1, Dictionary<string, (string Mode, string Sha1)> dict)
        {
            var entries = TreeUtils.GetTreeEntriesFromSha1(treeSha1);

            foreach (var entry in entries)
            {
                var fullPath = TreeUtils.CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    RecursiveReadTree(fullPath, entry.Sha1, dict);
                }
                else
                {
                    dict[fullPath] = (entry.Mode, entry.Sha1);
                }
            }
        }

        public static void RecursiveReadWorkSapce(string dir, Dictionary<string, string> dict)
        {
            foreach (var entry in Directory.GetFiles(dir))
            {
                if (WriteTree.ignoreFiles.Any(ignore => entry.Contains(ignore)))
                {
                    continue;
                }

                var fullPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), entry);
                dict[fullPath] = BlobUtils.GetSha1FromBlob(entry);
            }

            foreach (var entry in Directory.GetDirectories(dir))
            {
                if (WriteTree.ignoreFiles.Any(ignore => entry.Contains(ignore)))
                {
                    continue;
                }

                RecursiveReadWorkSapce(entry, dict);
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
