using System.Text;

namespace Csharp.Core
{
    public class TreeEntry
    {
        public required string Mode { get; set; }
        public required string Name { get; set; }
        public required string Sha1 { get; set; }
    }

    public class TreeObject
    {
        public static string WriteTree(List<TreeEntry> entries)
        {
            using var treeStream = new MemoryStream();

            foreach (var entry in entries)
            {
                var modeBytes = Encoding.ASCII.GetBytes(entry.Mode + " ");
                var nameBytes = Encoding.UTF8.GetBytes(entry.Name);
                var sha1Bytes = Utils.StringToSha1Bytes(entry.Sha1);

                treeStream.Write(modeBytes, 0, modeBytes.Length);
                treeStream.Write(nameBytes, 0, nameBytes.Length);
                treeStream.WriteByte(0); // separador \0
                treeStream.Write(sha1Bytes, 0, sha1Bytes.Length);
            }

            var treeContent = treeStream.ToArray();
            var header = $"tree {treeContent.Length}\0";
            var fullTree = Utils.CombineBytes(Encoding.UTF8.GetBytes(header), treeContent);

            var treeSha1 = Utils.ComputeSha1(fullTree);
            ObjectStore.WriteObject(treeSha1, fullTree);

            return treeSha1;
        }
    }
}
