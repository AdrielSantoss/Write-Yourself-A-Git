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

            var indexLines = CommitUtils.GetIndexEntries();
            if (!indexLines.Any())
            {
                throw new Exception("Nenhum arquivo na staging area.");
            }

            var trees = new Dictionary<string, List<TreeEntry>>();

            foreach (var line in indexLines)
            {
                var parts = line.Split(' ', 2);
                var sha1 = parts[0];
                var fullPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), parts[1]);
                var pathParts = fullPath.Split(Path.DirectorySeparatorChar);

                var fileName = pathParts.Last();
                var dirs = pathParts.Take(pathParts.Length - 1).ToArray();
                var key = string.Join(Path.DirectorySeparatorChar, dirs);

                if (!trees.ContainsKey(key))
                {
                    trees[key] = new List<TreeEntry>();
                }

                trees[key].Add(new TreeEntry
                {
                    Mode = "100644",
                    Name = fileName,
                    Sha1 = sha1
                });
            }

            string BuildTree(string path)
            {
                if (!trees.ContainsKey(path))
                {
                    trees[path] = new List<TreeEntry>();
                }

                var entries = new List<TreeEntry>(trees[path]);

                var subdirs = trees.Keys
                    .ToList()
                    .Where(k => k != path && k.StartsWith(path == "" ? "" : path + Path.DirectorySeparatorChar))
                    .Select(k =>
                    {
                        var remainder = path == "" ? k : k.Substring(path.Length + 1);
                        return remainder.Split(Path.DirectorySeparatorChar).First();
                    })
                    .Distinct();

                foreach (var subdir in subdirs)
                {
                    var subPath = path == "" ? subdir : Path.Combine(path, subdir);
                    var sha1 = BuildTree(subPath);

                    entries.Add(new TreeEntry
                    {
                        Mode = "040000",
                        Name = subdir,
                        Sha1 = sha1
                    });
                }

                return TreeObject.WriteTree(entries);
            }

            var rootSha1 = BuildTree("");

            var commitSha1 = CommitObject.WriteCommit(rootSha1, args[1]);
            UpdateHead(commitSha1);

            Console.WriteLine(commitSha1);
            CommitUtils.CreateOrUpdateIndex(string.Empty);

            return commitSha1;
        }

        private static void UpdateHead(string commitSha1)
        {
            var refs = BranchUtils.GetHead();
            var parts = refs.Split(" ", 2);

            BranchUtils.CreateOrUpdateBranch(parts[1], commitSha1);
        }
    }
}
