
using System.IO;
using System.IO.Compression;

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

            var hash = args[1];
            var dir = Path.Combine(".gitadr", "objects", hash.Substring(0, 2));
            var file = hash.Substring(2);
            var path = Path.Combine(dir, file);

            if (!Directory.Exists(dir) || !File.Exists(path))
            {
                Console.WriteLine($"Tree não encontrada: ${hash} não encontrado.");
                return;
            }

            using var fs = File.OpenRead(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            zlib.CopyTo(outputStream);
        }
    }
}
