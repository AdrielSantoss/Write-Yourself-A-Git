
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
            var author = Encoding.UTF8.GetBytes("author Adriel <adriel@gmail.com> 1723200000 -0300\n");
            var committer = Encoding.UTF8.GetBytes("committer Adriel <adriel@email.com> 1723200000 -0300\n");
            var message = Encoding.UTF8.GetBytes($"{args[1]}\n");

            commitStream.Write(tree, 0, tree.Length);
            commitStream.Write(author, 0, author.Length);
            commitStream.Write(committer, 0, committer.Length);
            commitStream.WriteByte(0x0A);
            commitStream.Write(message, 0, message.Length);

            var commitContent = commitStream.ToArray();
            var header = $"commit {commitContent.Length}\0";

            var fullCommit = Utils.CombineBytes(Encoding.UTF8.GetBytes(header), commitContent);
            var commitSha1 = Utils.ComputeSha1(fullCommit);

            Console.WriteLine(commitSha1);

            return string.Empty;
        }
    }
}
