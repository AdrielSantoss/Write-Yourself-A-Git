using System.IO.Compression;
using System.Text;

namespace Csharp.Commands
{
    public class CatFile
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-p")
            {
                Console.WriteLine("Uso: dotnet run -- cat-file [-p] <hash>");
                return;
            }

            string hash = args[1];

            string dir = Path.Combine(".gitadr", "objects", hash.Substring(0, 2));
            string file = hash.Substring(2);
            string path = Path.Combine(dir, file);

            if (!Directory.Exists(dir) || !File.Exists(path))
            {
                Console.WriteLine($"Objeto não encontrado: ${hash} não encontrado.");
                return;
            }

            using var fs = File.OpenRead(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            zlib.CopyTo(outputStream);
            var data = outputStream.ToArray();

            int nullIndex = Array.IndexOf(data, (byte)0);
            if (nullIndex == -1)
            {
                Console.WriteLine("Objeto inválido (sem header)");
                return;
            }

            Console.WriteLine(Encoding.UTF8.GetString(data[(nullIndex + 1)..]));
        }
    }
}
