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

        public static string[] GetIndexEntries(bool createIndexFile = false)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, string.Empty);
                }

                return Array.Empty<string>();
            }

            var content = File.ReadAllText(pathIndex);

            if (string.IsNullOrWhiteSpace(content))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, content);
                }

                return Array.Empty<string>();
            }

            return content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public static void CreateOrUpdateIndex(string contenet)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, "index");

            File.WriteAllText(path, contenet);
        }
    }
}
