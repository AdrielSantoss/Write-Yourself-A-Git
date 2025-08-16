namespace Git.Core
{
    public class BranchUtils
    {
        public static string GetHead()
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, "HEAD");

            return File.ReadAllText(path);
        }

        public static void WriteHead(string headContent)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, "HEAD");

            File.WriteAllText(path, headContent);
        }

        public static string? GetCommitHeadFromBranch(string branchFileName)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, "refs/heads", branchFileName);

            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }

        public static void CreateOrUpdateBranch(string branchPath, string commitSha1)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, branchPath);

            File.WriteAllText(path, commitSha1);
        }
    }
}
