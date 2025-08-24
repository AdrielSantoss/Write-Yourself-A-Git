using Git.Core;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            var head = BranchUtils.GetHead();
            Console.WriteLine(head.Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", "On branch ") + "\n");

            ExecuteRecursive(Directory.GetCurrentDirectory());
        }

        public static void ExecuteRecursive(string directory)
        {
            var worksSpaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());

            var commitHead = CommitUtils.GetLastCommitSha1FromHead();
            var headFiles = new Dictionary<string, (string Mode, string Sha1)>();

            if (!string.IsNullOrWhiteSpace(commitHead))
            {
                var commitTreeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);
                TreeUtils.GetTreeEntriesFromSha1("", commitTreeSha1, headFiles);
            }

            var indexFiles = IndexUtils.GetIndexEntries(false);

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
                Console.WriteLine("nothing to commit, working tree clean");
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
