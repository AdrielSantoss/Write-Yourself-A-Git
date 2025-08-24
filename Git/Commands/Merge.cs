using Csharp.Core;
using Git.Core;
using TreeEntry = Git.Core.TreeUtils.TreeEntry;

namespace Git.Commands
{
    public class Merge
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr merge <branch>");
                return;
            }

            var ffOnly = false;
            string targetBranch;

            if (args[0] == "--ff-only" && args.Length >= 2)
            {
                ffOnly = true;
                targetBranch = args[1];
            }
            else
            {
                targetBranch = args[0];
            }

            var targetCommit = BranchUtils.GetCommitHeadFromBranch(targetBranch);

            if (targetCommit == null)
            {
                Console.WriteLine($"Não existe um branch com o nome {targetBranch}");
                return;
            }

            var headBranch = BranchUtils.GetHead();
            var headCommit = CommitUtils.GetLastCommitSha1FromHead();
            var headCommits = BranchUtils.GetAllCommitsFromBranch(headBranch.Replace($"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", ""));

            var targetTreeSha1 = CommitUtils.GetCommitTreeSha1(targetCommit);
            var targetCommits = BranchUtils.GetAllCommitsFromBranch(targetBranch);

            if (targetCommits!.All(commit => headCommits!.Contains(commit)))
            {
                Console.WriteLine($"Already up to date.");

                return;
            }

            var commitBase = targetCommits!.Intersect(headCommits!).First();

            var baseTreeSha1 = CommitUtils.GetCommitTreeSha1(commitBase);
            var baseEntries = TreeUtils.GetTreeData(baseTreeSha1);

            var headTreeSha1 = CommitUtils.GetCommitTreeSha1(headCommit);
            var headEntries = TreeUtils.GetTreeData(headTreeSha1);

            var targetEntries = TreeUtils.GetTreeData(targetTreeSha1);

            var addedOrUpdatedFiles = new Dictionary<string, string>();
            var removedFiles = new Dictionary<string, string>();

            if (ffOnly)
            {
                var canFastForward = headCommits!.All(c => targetCommits!.Contains(c));

                if (!canFastForward)
                {
                    Console.WriteLine($"Não é possível realizar fast-forward merge para {targetBranch}, abortando.");
                    return;
                }

                UpdateIndexAndWorkSpaceFromTree(targetEntries);

                BranchUtils.CreateOrUpdateBranch(headBranch.Replace("ref: ", string.Empty), targetCommit);
                Console.WriteLine($"Fast-forward merge realizado para {targetBranch}");
                return;
            }

            var mergedEntries = BuildTreeFromTwoTreeDiffs(baseEntries, headEntries, targetEntries);

            if (mergedEntries.Count > 0)
            {
                UpdateIndexAndWorkSpaceFromTree(mergedEntries);

                var mergeRootSha1 = TreeObject.WriteTree(mergedEntries);
                var mergeCommitSha1 = CommitObject.WriteCommit(mergeRootSha1, $"Merge branch {targetBranch} into {headBranch.Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty)}", [headCommit, targetCommit]);

                BranchUtils.CreateOrUpdateBranch(headBranch.Replace("ref: ", string.Empty), mergeCommitSha1);
            }
        }

        public static List<TreeEntry> BuildTreeFromTwoTreeDiffs(
            List<TreeEntry> baseEntries,
            List<TreeEntry> headEntries,
            List<TreeEntry> targetEntries
        )
        {
            var mergedEntries = new List<TreeEntry>();

            var allNames = baseEntries.Select(e => e.Name)
                .Union(headEntries.Select(e => e.Name))
                .Union(targetEntries.Select(e => e.Name));

            foreach (var name in allNames)
            {
                var baseEntry = baseEntries.FirstOrDefault(e => e.Name == name);
                var headEntry = headEntries.FirstOrDefault(e => e.Name == name);
                var targetEntry = targetEntries.FirstOrDefault(e => e.Name == name);

                if (baseEntry == null && headEntry == null && targetEntry != null)
                {
                    mergedEntries.Add(targetEntry);
                    continue;
                }

                if (baseEntry == null && targetEntry == null && headEntry != null)
                {
                    mergedEntries.Add(headEntry);
                    continue;
                }

                if (headEntry == null && targetEntry == null && baseEntry != null)
                {
                    continue;
                }

                if (headEntry != null && targetEntry != null)
                {
                    if (headEntry.Mode.StartsWith("040") || targetEntry.Mode.StartsWith("040"))
                    {
                        var headSubtree = headEntry != null ? TreeUtils.GetTreeData(headEntry.Sha1) : new List<TreeEntry>();
                        var targetSubtree = targetEntry != null ? TreeUtils.GetTreeData(targetEntry.Sha1) : new List<TreeEntry>();
                        var baseSubtree = baseEntry != null ? TreeUtils.GetTreeData(baseEntry.Sha1) : new List<TreeEntry>();

                        var mergedSubtree = BuildTreeFromTwoTreeDiffs(baseSubtree, headSubtree, targetSubtree);

                        if (mergedSubtree.Any())
                        {
                            var newTreeSha1 = TreeObject.WriteTree(mergedSubtree);
                            mergedEntries.Add(new TreeEntry
                            {
                                Mode = "040000",
                                Name = name,
                                Sha1 = newTreeSha1
                            });
                        }

                        continue;
                    }

                    if (headEntry.Sha1 == targetEntry.Sha1)
                    {
                        mergedEntries.Add(headEntry);
                    }
                    else
                    {
                        Console.WriteLine($"Conflito detectado no arquivo {name}");
                        return new List<TreeEntry>();
                    }

                    continue;
                }
            }

            return mergedEntries;
        }

        private static void UpdateIndexAndWorkSpaceFromTree(List<TreeEntry> treeData)
        {
            var currentIndex = CommitUtils.GetIndexEntries(true);
            var survivingFiles = new HashSet<string>();

            foreach (var entry in treeData)
            {
                if (entry.Mode.StartsWith("040"))
                {
                    continue;
                }

                AddOrUpdateIndexFile(entry.Name, entry.Sha1);
                survivingFiles.Add(entry.Name);

                Sha1Utils.WriteFileAndDirectoriesFromSha1(entry.Name, entry.Sha1);
            }

            foreach (var file in currentIndex.Keys)
            {
                if (!survivingFiles.Contains(file))
                {
                    RemoveIndexFile(file);
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }

            CommitUtils.RemoveEmptyDirectories(Directory.GetCurrentDirectory());
        }

        private static void AddOrUpdateIndexFile(string file, string sha1)
        {
            var lines = CommitUtils.GetIndexEntries(true);

            var newContentLines = new List<string>();
            var found = false;

            foreach (var fileName in lines.Keys)
            {
                var fileSha1 = lines[fileName];

                if (fileName == file)
                {
                    found = true;
                    if (fileSha1 == sha1)
                    {
                        return;
                    }
                    else
                    {
                        newContentLines.Add($"{sha1} {fileName}");
                    }
                }
                else
                {
                    newContentLines.Add($"{fileSha1} {fileName}");
                }
            }

            if (!found)
            {
                newContentLines.Add($"{sha1} {file}");
            }

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }

        private static void RemoveIndexFile(string file)
        {
            var lines = CommitUtils.GetIndexEntries(true);
            lines.Remove(file);
            var newContentLines = new List<string>();

            foreach (var fileName in lines.Keys)
            {
                newContentLines.Add($"{lines[fileName]} {fileName}");
            }

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }
    }
}