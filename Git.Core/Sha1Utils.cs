using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Git.Core
{
    public class Sha1Utils
    {
        public static byte[] CombineBytes(byte[] a, byte[] b)
        {
            var combined = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, combined, 0, a.Length);
            Buffer.BlockCopy(b, 0, combined, a.Length, b.Length);

            return combined;
        }

        public static string Sha1BytesToString(byte[] sha1)
        {
            return BitConverter.ToString(sha1).Replace("-", "").ToLower();
        }

        public static byte[] Sha1StringToBytes(string sha1)
        {
            var bytes = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                var byteString = sha1.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteString, 16);
            }

            return bytes;
        }

        public static string CreateSha1FromByteData(byte[] data)
        {
            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(data);

            return Sha1BytesToString(hashBytes);
        }

        public static byte[] GetObjectDataBySha1(string sha1)
        {
            var dir = Path.Combine(".gitadr", "objects", sha1.Substring(0, 2));
            var file = sha1.Substring(2);
            var path = Path.Combine(dir, file);

            if (!Directory.Exists(dir) || !File.Exists(path))
            {
                throw new Exception($"Objeto não encontrado: ${sha1} não encontrado.");
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
