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

        public static List<string> GetCommitParents(string commitSha1)
        {
            var data = Sha1Utils.GetObjectDataBySha1(commitSha1);
            var nullIdx = Array.IndexOf(data, (byte)0);
            var content = data[(nullIdx + 1)..];

            var text = Encoding.UTF8.GetString(content);
            var parents = text.Split('\n').Where(l => l.StartsWith("parent "));

            var commits = new List<string>();

            foreach (var parent in parents)
            {
                commits.Add(parent.Split(" ", 2)[1]);
            }

            return commits;
        }
    }
}
