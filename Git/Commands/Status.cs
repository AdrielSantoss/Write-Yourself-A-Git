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
            var filesStatus = new Dictionary<string, string>();

            foreach (var file in worksSpaceFiles.Keys)
            {
                filesStatus.Add(file, $"Untracked: {file}");
            }

            if (!string.IsNullOrWhiteSpace(commitHead))
            {
                var commitTreeSha1 = CommitUtils.GetCommitTreeSha1(commitHead);
                var headFiles = new Dictionary<string, (string Mode, string Sha1)>();

                RecursiveReadTree("", commitTreeSha1, headFiles);

                foreach (var file in worksSpaceFiles.Keys)
                {
                    var sha1 = worksSpaceFiles[file];

                    if (headFiles.Keys.Contains(file))
                    {
                        if (headFiles[file].Sha1 == sha1)
                        {
                            filesStatus.Remove(file);
                            continue;
                        }

                        filesStatus[file] = $"Modified: {file}";
                    }
                    else
                    {
                        filesStatus[file] = $"Untracked: {file}";
                    }
                }
            }

            var indexFiles = CommitUtils.GetIndexEntries(false);

            foreach (var file in indexFiles.Keys)
            {
                var sha1 = indexFiles[file];

                if (worksSpaceFiles.Keys.Contains(file))
                {
                    if (worksSpaceFiles[file] == sha1)
                    {
                        filesStatus[file] = $"Staged: {file}";
                    }
                    else
                    {
                        filesStatus[file] = $"Modified: {file}";
                    }
                }
            }

            foreach (var key in filesStatus.Keys)
            {
                var status = filesStatus[key];
                if (status.Contains("Staged:"))
                {
                    ConsoleWithColor(status, ConsoleColor.Green);
                    continue;
                }

                ConsoleWithColor(status, ConsoleColor.Red);
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
            var entries = Directory.GetFiles(dir);

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
