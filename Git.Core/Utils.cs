using System.Security.Cryptography;
using System.Text;

namespace Csharp.Core
{
    public class Utils
    {

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
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public static string GetSha1FromBlob(string path)
        {
            byte[] content = File.ReadAllBytes(path);
            byte[] header = Encoding.UTF8.GetBytes($"blob {content.Length}\0");
            byte[] fullBlob = CombineBytes(header, content);

            using var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(fullBlob);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public static (string sha1, byte[] fullBlob) WriteBlob(string filePath)
        {
            var content = File.ReadAllBytes(filePath);
            var header = $"blob {content.Length}\0";
            var fullBlob = CombineBytes(Encoding.UTF8.GetBytes(header), content);
            var sha1 = ComputeSha1(fullBlob);

            return (sha1, fullBlob);
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
    }
}
