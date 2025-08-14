using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Csharp.Core
{
    public class Utils
    {
        public static string Sha1BytesToString(byte[] sha1)
        {
            return BitConverter.ToString(sha1).Replace("-", "").ToLower();
        }

        public static byte[] StringToSha1Bytes(string sha1)
        {
            var bytes = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                var byteString = sha1.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteString, 16);
            }
            return bytes;
        }

        public static byte[] CombineBytes(byte[] a, byte[] b)
        {
            var combined = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, combined, 0, a.Length);
            Buffer.BlockCopy(b, 0, combined, a.Length, b.Length);
            return combined;
        }

        public static string ComputeSha1(byte[] data)
        {
            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(data);

            return Sha1BytesToString(hashBytes);
        }

        public static string GetSha1FromBlob(string path)
        {
            byte[] content = File.ReadAllBytes(path);
            byte[] header = Encoding.UTF8.GetBytes($"blob {content.Length}\0");
            byte[] fullBlob = CombineBytes(header, content);

            using var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(fullBlob);

            return Sha1BytesToString(hashBytes);
        }

        public static string GetSha1FromTree(string path)
        {
            byte[] content = File.ReadAllBytes(path);
            byte[] header = Encoding.UTF8.GetBytes($"blob {content.Length}\0");
            byte[] fullBlob = CombineBytes(header, content);

            using var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(fullBlob);

            return Sha1BytesToString(hashBytes);
        }

        public static (string sha1, byte[] fullBlob) WriteBlob(string filePath)
        {
            var content = File.ReadAllBytes(filePath);
            var header = $"blob {content.Length}\0";
            var fullBlob = CombineBytes(Encoding.UTF8.GetBytes(header), content);
            var sha1 = ComputeSha1(fullBlob);

            return (sha1, fullBlob);
        }

        public static byte[] GetObjectDataBySha1(string hash)
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
        public static string GetTimestamp() => DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        public static string GetTimezone() => DateTimeOffset.Now.ToString("zzz").Replace(":", "");

        public static string ReadLastCommitSha1()
        {
            var gitAdrDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            return File.ReadAllText(Path.Combine(gitAdrDir, "refs", "heads", "master"));
        }

        public static string[] GetIndexFileContentLines(bool createIndexFile = false)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            if (!File.Exists(pathIndex))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, string.Empty);
                }
                    

                return Array.Empty<string>();
            }

            var content = File.ReadAllText(pathIndex);

            if (string.IsNullOrWhiteSpace(content))
            {
                if (createIndexFile)
                {
                    File.WriteAllText(pathIndex, content);
                }

                return Array.Empty<string>();
            }

            return content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }


        public static void WriteIndexFile(string newContenet)
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");
            var pathIndex = Path.Combine(gitDir, "index");

            File.WriteAllText(pathIndex, newContenet);
        }
    }
}
