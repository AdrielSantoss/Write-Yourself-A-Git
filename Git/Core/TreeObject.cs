using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //public static string WriteTree(List<TreeEntry> entries)
        //{
        //    using var treeStream = new MemoryStream();

        //    foreach (var entry in entries)
        //    {
        //        byte[] modeBytes = Encoding.ASCII.GetBytes(entry.Mode + " ");
        //        byte[] nameBytes = Encoding.UTF8.GetBytes(entry.Name);
        //        byte[] sha1Bytes = StringToSha1Bytes(entry.Sha1);

        //        treeStream.Write(modeBytes, 0, modeBytes.Length);
        //        treeStream.Write(nameBytes, 0, nameBytes.Length);
        //        treeStream.WriteByte(0);
        //        treeStream.Write(sha1Bytes, 0, sha1Bytes.Length);
        //    }

        //    byte[] treeContent = treeStream.ToArray();
        //    string header = $"tree {treeContent.Length}\0";
        //    byte[] fullTree = Combine(Encoding.UTF8.GetBytes(header), treeContent);

        //    string treeSha1 = ComputeSha1(fullTree);
        //    ObjectStore.WriteObject(treeSha1, fullTree);

        //    return treeSha1;
        //}
    }
}
