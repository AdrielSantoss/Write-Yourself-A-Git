using System.Text;

namespace Git.Core
{
    public class TreeUtils
    {
        public class TreeEntry
        {
            public required string Mode { get; set; }
            public required string Name { get; set; }
            public required string Sha1 { get; set; }
        }

        public static List<TreeEntry> GetTreeData(string sha1Param)
        {
            var data = Sha1Utils.GetObjectDataBySha1(sha1Param);

            var nullIndexHeader = Array.IndexOf(data, (byte)0);
            var content = data.Skip(nullIndexHeader + 1).ToArray();

            var entries = new List<TreeEntry>();
            int offset = 0;

            while (offset < content.Length)
            {
                int modeEnd = Array.IndexOf(content, (byte)0x20, offset);
                var mode = Encoding.UTF8.GetString(content, offset, modeEnd - offset);

                int nameEnd = Array.IndexOf(content, (byte)0, modeEnd + 1);
                var name = Encoding.UTF8.GetString(content, modeEnd + 1, nameEnd - (modeEnd + 1));

                var sha1Bytes = content.Skip(nameEnd + 1).Take(20).ToArray();
                var sha1 = Sha1Utils.Sha1BytesToString(sha1Bytes);

                entries.Add(new TreeEntry { Mode = mode, Sha1 = sha1, Name = name });

                offset = nameEnd + 1 + 20;
            }

            return entries;
        }

        public static Dictionary<string, (string Mode, string Sha1)> GetTreeEntriesFromSha1(string prefix, string treeSha1, Dictionary<string, (string Mode, string Sha1)> dict)
        {
            var entries = GetTreeData(treeSha1);

            foreach (var entry in entries)
            {
                var fullPath = TreeUtils.CombinePrefix(prefix, entry.Name);

                if (entry.Mode == "040000")
                {
                    GetTreeEntriesFromSha1(fullPath, entry.Sha1, dict);
                }
                else
                {
                    dict[fullPath] = (entry.Mode, entry.Sha1);
                }
            }

            return dict;
        }

        public static string CombinePrefix(string prefix, string name)
        {
            return string.IsNullOrEmpty(prefix) ? name : Path.Combine(prefix, name);
        }
    }
}
