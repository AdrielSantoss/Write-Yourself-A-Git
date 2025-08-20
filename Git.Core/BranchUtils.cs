using System.IO;
using System.Text;

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

        public static List<string>? GetAllCommitsFromBranch(string branchFileName)
        {
            var head = GetCommitHeadFromBranch(branchFileName);

            if (string.IsNullOrWhiteSpace(head))
            {
                return null;
            }

            void GetParentCommit(string sha1, List<string> commits)
            {
                var data = Sha1Utils.GetObjectDataBySha1(sha1);
                var nullIndex = Array.IndexOf(data, (byte)0);
                var content = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

                var lines = content.Split('\n');

                var parent = lines.FirstOrDefault(line => line.StartsWith("parent "));

                if (!string.IsNullOrWhiteSpace(parent))
                {
                    var commitParent = parent.Split(" ")[1];
                    commits.Add(commitParent);

                    GetParentCommit(commitParent, commits);
                }
            }

            var listCommits = new List<string>() { head };

            GetParentCommit(head, listCommits);

            return listCommits;
        }

        public static void CreateOrUpdateBranch(string branchPath, string commitSha1)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var path = Path.Combine(gitDir, branchPath);

            File.WriteAllText(path, commitSha1);
        }
    }
}
