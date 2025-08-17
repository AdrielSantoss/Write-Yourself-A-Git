using System.Text;

namespace Git.Core
{
    public class CommitUtils
    {
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
    }
}
