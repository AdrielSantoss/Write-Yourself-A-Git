using System;
using System.IO.Compression;
using System.Text;

namespace Csharp.Core
{
    public class ObjectStore
    {
        public static void WriteObject(string sha1, byte[] data)
        {
            string dir = Path.Combine(".gitadr", "objects", sha1.Substring(0, 2));
            string file = sha1.Substring(2);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path = Path.Combine(dir, file);
            using var fs = File.Create(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Compress);
            zlib.Write(data, 0, data.Length);
        }

        public static byte[] ReadObject(string hash)
        {
            var dir = Path.Combine(".gitadr", "objects", hash.Substring(0, 2));
            var file = hash.Substring(2);
            var path = Path.Combine(dir, file);

            if (!Directory.Exists(dir) || !File.Exists(path))
            {
                throw new Exception($"Objeto não encontrado: ${hash} não encontrado.");
            }

            using var fs = File.OpenRead(path);
            using var zlib = new ZLibStream(fs, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();

            zlib.CopyTo(outputStream);
            var data = outputStream.ToArray();

            var nullIndex = Array.IndexOf(data, (byte)0);
            if (nullIndex == -1)
            {
                throw new Exception("Objeto inválido (sem header)");
            }

            return data;
        }
    }
}
