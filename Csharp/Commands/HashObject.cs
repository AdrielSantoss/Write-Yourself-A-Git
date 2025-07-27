using Csharp.Core;
using System.Security.Cryptography;
using System.Text;

namespace Csharp.Commands
{
    public class HashObject
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Uso: dotnet run -- hash-object [-w] <arquivo>");
                return;
            }

            var write = args[0] == "-w";
            var path = write ? args[1] : args[0];

            if (!File.Exists(path))
            {
                Console.WriteLine($"Arquivo não encontrado: {path}");
                return;
            }

            var content = File.ReadAllBytes(path);
            var header = $"blob {content.Length}\0";
            var fullBlob = CreateBlob(Encoding.UTF8.GetBytes(header), content);

            var sha1Hash = ComputeSha1(fullBlob);
            Console.WriteLine(sha1Hash);

            if (write)
            {
                ObjectStore.WriteObject(sha1Hash, fullBlob);
            }
        }

        private static byte[] CreateBlob(byte[] a, byte[] b)
        {
            var combined = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, combined, 0, a.Length);
            Buffer.BlockCopy(b, 0, combined, a.Length, b.Length);
            return combined;
        }

        private static string ComputeSha1(byte[] data)
        {
            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
