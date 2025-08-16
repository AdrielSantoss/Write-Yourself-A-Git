using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Git.Core
{
    public class BlobUtils
    {
        public static string GetSha1FromBlob(string blobPath)
        {
            byte[] content = File.ReadAllBytes(blobPath);
            byte[] header = Encoding.UTF8.GetBytes($"blob {content.Length}\0");
            byte[] fullBlob = Sha1Utils.CombineBytes(header, content);

            using var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(fullBlob);

            return Sha1Utils.Sha1BytesToString(hashBytes);
        }

        public static (string sha1, byte[] fullBlob) WriteBlob(string filePath)
        {
            var content = File.ReadAllBytes(filePath);
            var header = $"blob {content.Length}\0";
            var fullBlob = Sha1Utils.CombineBytes(Encoding.UTF8.GetBytes(header), content);
            var sha1 = Sha1Utils.CreateSha1FromByteData(fullBlob);

            return (sha1, fullBlob);
        }
    }
}
