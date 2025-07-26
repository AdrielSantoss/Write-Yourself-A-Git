using System.Security.Cryptography;
using System.Text;

namespace Csharp.Core
{
    public class Utils
    {
        public static string GetSha1FromBlob(string path)
        {
            byte[] content = File.ReadAllBytes(path);
            byte[] header = Encoding.UTF8.GetBytes($"blob {content.Length}\0");
            byte[] fullBlob = new byte[header.Length + content.Length];

            Buffer.BlockCopy(header, 0, fullBlob, 0, header.Length);
            Buffer.BlockCopy(content, 0, fullBlob, header.Length, content.Length);

            using var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(fullBlob);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
