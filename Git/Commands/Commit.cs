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

            var indexEntries = CommitUtils.GetIndexEntries();

            if (indexEntries == null || indexEntries.Length == 0)
            {
                throw new Exception("Nenhum arquivo na staging area.");
            }

            var parentCommit = CommitUtils.GetLastCommitSha1FromHead();
                    
            var rootSha1 = BuildCommitTree(indexEntries, parentCommit);

            var commitSha1 = CommitObject.WriteCommit(rootSha1, args[1]);

            UpdateHead(commitSha1);

            Console.WriteLine(commitSha1);

            CommitUtils.CreateOrUpdateIndex(string.Empty);

            return commitSha1;
        }

        private static string BuildCommitTree(string[] index, string? parentCommit)
        {
            var indexMap = StringIndexEntriesToDictonary(index);
            var baseFiles = new Dictionary<string, (string Mode, string Sha1)>();

            if (!string.IsNullOrEmpty(parentCommit))
            {
                var parentTreeSha1 = CommitUtils.GetCommitTreeSha1(parentCommit);
                RecursiveExpandTree("", parentTreeSha1, baseFiles);
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
                    var fullPath = CombinePrefix(prefix, name);

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

        private static void RecursiveExpandTree(string prefix, string treeSha1, Dictionary<string, (string Mode, string Sha1)> dict)
        {
            var entries = TreeUtils.GetTreeEntriesFromSha1(treeSha1);

            foreach (var entry in entries)
            {
                var fullPath = CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    RecursiveExpandTree(fullPath, entry.Sha1, dict);
                }
                else
                {
                    dict[fullPath] = (entry.Mode, entry.Sha1);
                }
            }
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

        private static string CombinePrefix(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : Path.Combine(prefix, name);
        }

        private static Dictionary<string, string> StringIndexEntriesToDictonary(string[] entries)
        {
            var indexMap = new Dictionary<string, string>();

            foreach (var line in entries)
            {
                var parts = line.Split(' ', 2);

                var sha1 = parts[0];
                var path = parts[1];

                var relPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);

                relPath = relPath.Replace('/', Path.DirectorySeparatorChar);

                indexMap[relPath] = sha1;
            }

            return indexMap;
        }

        private static void UpdateHead(string commitSha1)
        {
            var refs = BranchUtils.GetHead();
            var parts = refs.Split(" ", 2);
            BranchUtils.CreateOrUpdateBranch(parts[1], commitSha1);
        }
    }
}
