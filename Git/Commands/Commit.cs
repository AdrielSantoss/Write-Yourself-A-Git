using Csharp.Core;
using Git.Core;

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

            var indexEntries = CommitUtils.GetIndexEntries(false);

            if (indexEntries == null || indexEntries.Keys.Count == 0)
            {
                Console.WriteLine("Nenhum arquivo na staging area.");
                return string.Empty;
            }

            var parentCommit = CommitUtils.GetLastCommitSha1FromHead();

            if (!string.IsNullOrEmpty(parentCommit))
            {
                var headTreeSha1 = CommitUtils.GetCommitTreeSha1(parentCommit);
                var headEntries = TreeUtils.GetTreeEntriesFromSha1("", headTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

                var hasChanges = false;

                if (indexEntries.Count != headEntries.Count)
                {
                    hasChanges = true;
                }
                else
                {
                    foreach (var kv in indexEntries)
                    {
                        if (!headEntries.TryGetValue(kv.Key, out var head) || head.Sha1 != kv.Value)
                        {
                            hasChanges = true;
                            break;
                        }
                    }
                }

                if (!hasChanges)
                {
                    Console.WriteLine("nothing to commit, working tree clean");
                    return string.Empty;
                }
            }
                    
            var rootSha1 = BuildCommitTree(indexEntries, parentCommit);

            var commitSha1 = CommitObject.WriteCommit(rootSha1, args[1]);

            UpdateHead(commitSha1);

            Console.WriteLine(commitSha1);

            return commitSha1;
        }

        private static string BuildCommitTree(Dictionary<string, string> indexMap, string? parentCommit)
        {
            var baseFiles = new Dictionary<string, (string Mode, string Sha1)>();

            if (!string.IsNullOrEmpty(parentCommit))
            {
                var parentTreeSha1 = CommitUtils.GetCommitTreeSha1(parentCommit);
                TreeUtils.GetTreeEntriesFromSha1("", parentTreeSha1, baseFiles);
            }

            foreach (var keyValue in indexMap)
            {
                var relPath = keyValue.Key;
                var blobSha1 = keyValue.Value;
                baseFiles[relPath] = ("100644", blobSha1);
            }

            string WriteDir(string prefix)
            {
                var childNames = baseFiles.Keys
                    .Where(p => IsUnderPrefix(p, prefix))
                    .Select(p =>
                    {
                        var remainder = GetRemainder(p, prefix);
                        return remainder.Split(Path.DirectorySeparatorChar)[0];
                    })
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();

                var entries = new List<TreeEntry>();

                foreach (var name in childNames)
                {
                    var fullPath = TreeUtils.CombinePrefix(prefix, name);

                    if (baseFiles.TryGetValue(fullPath, out var fileInfo))
                    {
                        entries.Add(new TreeEntry
                        {
                            Mode = fileInfo.Mode,
                            Name = name,
                            Sha1 = fileInfo.Sha1
                        });
                    }
                    else
                    {
                        var subTreeSha1 = WriteDir(fullPath);
                        entries.Add(new TreeEntry
                        {
                            Mode = "040000",
                            Name = name,
                            Sha1 = subTreeSha1
                        });
                    }
                }

                return TreeObject.WriteTree(entries);
            }

            return WriteDir("");
        }

        private static bool IsUnderPrefix(string path, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return true;
            }

            if (path.Length < prefix.Length)
            {
                return false;
            }

            if (!path.StartsWith(prefix)) 
            { 
                return false; 
            }

            if (path.Length == prefix.Length) 
            { 
                return false; 
            }

            return path[prefix.Length] == Path.DirectorySeparatorChar;
        }

        private static string GetRemainder(string path, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return path;
            }

            return path.Substring(prefix.Length + 1);
        }

        private static void UpdateHead(string commitSha1)
        {
            var refs = BranchUtils.GetHead();
            var parts = refs.Split(" ", 2);
            BranchUtils.CreateOrUpdateBranch(parts[1], commitSha1);
        }
    }
}
