
using Csharp.Core;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Git.Commands
{
    public class LsTree
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-p")
            {
                Console.WriteLine("Uso: dotnet run -- ls-tree [-p] <hash>");
                return;
            }

            var data = Utils.GetObjectDataBySha1(args[1]);
            var nullIndexHeader = Array.IndexOf(data, (byte)0);
            var content = data.Skip(nullIndexHeader + 1).ToArray();

            int offset = 0;
            while (offset < content.Length)
            {
                int modeEnd = Array.IndexOf(content, (byte)0x20, offset);
                var mode = Encoding.UTF8.GetString(content, offset, modeEnd - offset);

                int nameEnd = Array.IndexOf(content, (byte)0, modeEnd + 1);
                var name = Encoding.UTF8.GetString(content, modeEnd + 1, nameEnd - (modeEnd + 1));

                var sha1Bytes = content.Skip(nameEnd + 1).Take(20).ToArray();
                var sha1 = Utils.Sha1BytesToString(sha1Bytes);

                var type = mode == "040000" ? "tree" : "blob";

                Console.WriteLine($"{mode} {type} {sha1} {name}");

                offset = nameEnd + 1 + 20;
            }
        }
    }
}
