
using Csharp.Commands;
using Csharp.Core;
using System.Text;

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

            var rootSha1 = WriteTree.Execute();
            using var commitStream = new MemoryStream();

            var tree = Encoding.UTF8.GetBytes($"tree {rootSha1}\n");
            var parentSah1 = Utils.ReadLastCommitSha1();
            var parent = !string.IsNullOrWhiteSpace(parentSah1) ? Encoding.UTF8.GetBytes($"parent {parentSah1}\n") : null;
            var author = Encoding.UTF8.GetBytes($"author Guest <author@gmail.com> {Utils.GetTimestamp()} {Utils.GetTimezone()}\n");
            var committer = Encoding.UTF8.GetBytes($"committer Guest <commiter@email.com> {Utils.GetTimestamp()} {Utils.GetTimezone()}\n");
            var message = Encoding.UTF8.GetBytes($"{args[1]}\n");

            commitStream.Write(tree, 0, tree.Length);

            if(parent is not null)
            {
                commitStream.Write(parent, 0, parent.Length);
            }

            commitStream.Write(author, 0, author.Length);
            commitStream.Write(committer, 0, committer.Length);
            commitStream.WriteByte(0x0A);
            commitStream.Write(message, 0, message.Length);

            var commitContent = commitStream.ToArray();
            var header = $"commit {commitContent.Length}\0";

            var fullCommit = Utils.CombineBytes(Encoding.UTF8.GetBytes(header), commitContent);
            var commitSha1 = Utils.ComputeSha1(fullCommit);

            ObjectStore.WriteObject(commitSha1, fullCommit);
            UpdateHead(commitSha1);

            Console.WriteLine(commitSha1);

            return string.Empty;
        }

        private static void UpdateHead(string commitSha1)
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            File.WriteAllText(Path.Combine(gitAdrDir, "refs", "heads", "master"), commitSha1);
        }
    }
}
