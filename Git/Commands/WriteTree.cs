using Csharp.Core;
using System.Reflection.Metadata;
using System.Text;

namespace Csharp.Commands
{
    public class WriteTree
    {

        private static string[] ignoreFiles = { ".gitadr", "Program.cs", "Git.csproj" };

        public static void Execute(string[] args)
        {
            var entries = new List<TreeEntry>();

            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (ignoreFiles.Any(ignore => file.Contains(ignore)))
                {
                    continue;
                }

                var (blobSha1, fullBlob) = Utils.WriteBlob(file);
                ObjectStore.WriteObject(blobSha1, fullBlob);

                entries.Add(new TreeEntry
                {
                    Mode = "100644",
                    Name = Path.GetFileName(file),
                    Sha1 = blobSha1
                });
            }

            string treeSha1 = TreeObject.WriteTree(entries);
            Console.WriteLine(treeSha1);
        }
    }
}
