using Csharp.Core;
using System.Collections.Generic;
using System.Text;

namespace Csharp.Commands
{
    public class WriteTree
    {
        public static readonly string[] ignoreFiles = { ".gitadr", "Program.cs", "Git.csproj", "bin", "Commands", "Core", "obj", ".vs" };

        public static string Execute()
        {
            return WriteTreeRecursive(Directory.GetCurrentDirectory());
        }

        public static string WriteTreeRecursive(string directory)
        {
            var entries = new List<TreeEntry>();

            foreach (var file in Directory.GetFiles(directory))
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

            foreach (var subdir in Directory.GetDirectories(directory))
            {
                if (ignoreFiles.Any(ignore => subdir.Contains(ignore)))
                {
                    continue;
                }

                var treeSha1 = WriteTreeRecursive(subdir);

                entries.Add(new TreeEntry
                {
                    Mode = "040000",
                    Name = Path.GetFileName(subdir),
                    Sha1 = treeSha1
                });
            }

            return TreeObject.WriteTree(entries);
        }
    }
}
