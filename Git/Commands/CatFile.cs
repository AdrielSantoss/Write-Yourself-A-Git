using System.IO.Compression;
using System.Text;

namespace Csharp.Commands
{
    public class CatFile
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-p")
            {
                Console.WriteLine("Uso: dotnet run -- cat-file [-p] <hash>");
                return string.Empty;
            }

            var hash = args[1];

            var dir = Path.Combine(".gitadr", "objects", hash.Substring(0, 2));
            var file = hash.Substring(2);
            var path = Path.Combine(dir, file);

            if (!Directory.Exists(dir) || !File.Exists(path))
            {
                Console.WriteLine($"Objeto não encontrado: ${hash} não encontrado.");
                return string.Empty;
            }

            using var fs = File.OpenRead(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            zlib.CopyTo(outputStream);
            var data = outputStream.ToArray();

            var nullIndex = Array.IndexOf(data, (byte)0);
            if (nullIndex == -1)
            {
                Console.WriteLine("Objeto inválido (sem header)");
                return string.Empty;
            }

            var content = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);
            Console.WriteLine(content);

            return content;
        }
    }
}
