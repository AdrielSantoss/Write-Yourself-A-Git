using Csharp.Core;
using System.Text;

namespace Git.Core
{
    public class CommitObject
    {
        public static string WriteCommit(string rootSha1, string msg, string[]? sha1Parents = null)
        {
            using var commitStream = new MemoryStream();

            var tree = Encoding.UTF8.GetBytes($"tree {rootSha1}\n");

            var parents = new List<byte[]>();

            if (sha1Parents != null) 
            {
                foreach (var sha1 in sha1Parents)
                {
                    parents.Add(Encoding.UTF8.GetBytes($"parent {sha1}\n"));
                }
            }
            else
            {
                var parentSah1 = CommitUtils.GetLastCommitSha1FromHead();
               
                if (!string.IsNullOrWhiteSpace(parentSah1))
                {
                    parents.Add(Encoding.UTF8.GetBytes($"parent {parentSah1}\n"));
                }
            }

            var author = Encoding.UTF8.GetBytes($"author Guest <author@gmail.com> {CommitUtils.GetTimestamp()} {CommitUtils.GetTimezone()}\n");
            var committer = Encoding.UTF8.GetBytes($"committer Guest <commiter@email.com> {CommitUtils.GetTimestamp()} {CommitUtils.GetTimezone()}\n");
            var message = Encoding.UTF8.GetBytes($"{msg}\n");

            commitStream.Write(tree, 0, tree.Length);

            foreach(var parent in parents)
            {
                commitStream.Write(parent, 0, parent.Length);
            }  

            commitStream.Write(author, 0, author.Length);
            commitStream.Write(committer, 0, committer.Length);
            commitStream.WriteByte(0x0A);
            commitStream.Write(message, 0, message.Length);

            var commitContent = commitStream.ToArray();
            var header = $"commit {commitContent.Length}\0";

            var fullCommit = Sha1Utils.CombineBytes(Encoding.UTF8.GetBytes(header), commitContent);
            var commitSha1 = Sha1Utils.CreateSha1FromByteData(fullCommit);

            ObjectStore.WriteObject(commitSha1, fullCommit);

            return commitSha1;
        }
    }
}
