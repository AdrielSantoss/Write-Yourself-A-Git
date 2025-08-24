using System.Text;

namespace Git.Core
{
    public class CommitUtils
    {
        public static readonly string[] ignoreFiles = { ".gitadr", "Program.cs", "Git.csproj", "bin", "Commands", "Core", "obj", ".vs" };
        public static string GetTimestamp() => DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        public static string GetTimezone() => DateTimeOffset.Now.ToString("zzz").Replace(":", "");

        public static string GetLastCommitSha1FromHead()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var refs = BranchUtils.GetHead();
            var parts = refs.Split(" ", 2);

            return File.ReadAllText(Path.Combine(gitAdrDir, parts[1]));
        }

        public static string GetCommitTreeSha1(string commitSha1)
        {
            var data = Sha1Utils.GetObjectDataBySha1(commitSha1);
            var nullIdx = Array.IndexOf(data, (byte)0);
            var content = data[(nullIdx + 1)..];

            var text = Encoding.UTF8.GetString(content);
            var lineTree = text.Split('\n').First(l => l.StartsWith("tree "));
                
            var parts = lineTree.Split(' ', 2);

            return parts[1];
        }

        public static Dictionary<string, string> GetIndexEntries(bool createIndexFile = false)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, string.Empty);
                }

                return new Dictionary<string, string>();
            }

            var content = File.ReadAllText(pathIndex);

            if (string.IsNullOrWhiteSpace(content))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, content);
                }

                return new Dictionary<string, string>();
            }

            var entries = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

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

        public static void CreateOrUpdateIndex(string contenet)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, "index");

            File.WriteAllText(path, contenet);
        }

        public static Dictionary<string, string> RecursiveReadWorkSapce(string dir, Dictionary<string, string> dict)
        {
            foreach (var entry in Directory.GetFiles(dir))
            {
                if (ignoreFiles.Any(ignoreFile => Path.GetFileName(entry) == ignoreFile))
                {
                    continue;
                }

                var fullPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), entry);
                dict[fullPath] = BlobUtils.GetSha1FromBlob(entry);
            }

            foreach (var entry in Directory.GetDirectories(dir))
            {
                if (ignoreFiles.Any(ignoreDir => Path.GetFileName(entry) == ignoreDir))
                {
                    continue;
                }

                RecursiveReadWorkSapce(entry, dict);
            }

            return dict;
        }

        public static List<string> RecursiveUpdateIndexFromTree(string prefix, string treeSha1, List<string> indexLines)
        {
            var treeEntries = TreeUtils.GetTreeData(treeSha1);

            foreach (var entry in treeEntries)
            {
                var fullPath = TreeUtils.CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    RecursiveUpdateIndexFromTree(fullPath, entry.Sha1, indexLines);
                }
                else
                {
                    indexLines.Add($"{entry.Sha1} {fullPath}");
                }
            }

            CreateOrUpdateIndex(string.Join('\n', indexLines) + "\n");

            return indexLines;
        }

        public static string NormalizePath(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var repoRoot = Directory.GetParent(Path.Combine(Directory.GetCurrentDirectory(), ".gitadr"))!.FullName;
            var relativePath = Path.GetRelativePath(repoRoot, fullPath);

            return relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
        }

        public static void RemoveEmptyDirectories(string root)
        {
            var allDirs = Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
                                   .OrderByDescending(d => d.Length);

            var gitDir = Path.GetFullPath(Path.Combine(root, ".gitadr"));

            foreach (var dir in allDirs)
            {
                var full = Path.GetFullPath(dir);
                if (full.Equals(gitDir, StringComparison.OrdinalIgnoreCase) || full.StartsWith(gitDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!Directory.EnumerateFileSystemEntries(full).Any())
                {
                    Directory.Delete(full, recursive: false);
                }
            }
        }
    }
}
