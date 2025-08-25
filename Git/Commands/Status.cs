using Git.Core;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            var head = BranchUtils.GetHead();
            Console.WriteLine(head.Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", "On branch ") + "\n");

            ExecuteRecursive();
        }

        public static void ExecuteRecursive()
        {
            var workspaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());
            var indexFiles = IndexUtils.GetIndexEntries(false);

            var commitHead = CommitUtils.GetLastCommitSha1FromHead();
            var headFiles = new Dictionary<string, (string Mode, string Sha1)>();
            if (!string.IsNullOrWhiteSpace(commitHead))
            {
                var commitTreeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);
                TreeUtils.GetTreeEntriesFromSha1("", commitTreeSha1, headFiles);
            }

            var allFiles = workspaceFiles.Keys
                .Union(indexFiles.Keys)
                .Union(headFiles.Keys)
                .ToHashSet();

            var staged = new List<string>();
            var modified = new List<string>();
            var deleted = new List<string>();
            var untracked = new List<string>();

            foreach (var file in allFiles)
            {
                headFiles.TryGetValue(file, out var headEntry);
                var headSha1 = headEntry.Sha1;
                indexFiles.TryGetValue(file, out var indexSha1);
                workspaceFiles.TryGetValue(file, out var wsSha1);

                if (headSha1 != indexSha1)
                {
                    if (headSha1 == null && indexSha1 != null)
                    {
                        staged.Add($"new file: {file}");
                    }
                    else if (headSha1 != null && indexSha1 == null)
                    {
                        staged.Add($"deleted: {file}");
                    }
                    else
                    {
                        staged.Add($"modified: {file}");
                    }
                }

                if (wsSha1 != null && indexSha1 != null && wsSha1 != indexSha1)
                {
                    modified.Add(file);
                }
                    
                if (indexSha1 != null && wsSha1 == null)
                {
                    deleted.Add(file);
                }
                    
                if (headSha1 == null && indexSha1 == null && wsSha1 != null)
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
