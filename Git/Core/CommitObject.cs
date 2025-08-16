using Csharp.Core;
using System.Text;

namespace Git.Core
{
    public class CommitObject
    {
        public static string WriteCommit(string rootSha1, string msg)
        {
            using var commitStream = new MemoryStream();

            var tree = Encoding.UTF8.GetBytes($"tree {rootSha1}\n");

            var parentSah1 = CommitUtils.GetLastCommitSha1FromHead();
            var parent = !string.IsNullOrWhiteSpace(parentSah1) ? Encoding.UTF8.GetBytes($"parent {parentSah1}\n") : null;
            var author = Encoding.UTF8.GetBytes($"author Guest <author@gmail.com> {CommitUtils.GetTimestamp()} {CommitUtils.GetTimezone()}\n");
            var committer = Encoding.UTF8.GetBytes($"committer Guest <commiter@email.com> {CommitUtils.GetTimestamp()} {CommitUtils.GetTimezone()}\n");
            var message = Encoding.UTF8.GetBytes($"{msg}\n");

            commitStream.Write(tree, 0, tree.Length);

            if (parent is not null)
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
