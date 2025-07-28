using Csharp.Core;
using System.Reflection.Metadata;
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

            var (sha1Hash, fullBlob) = Utils.WriteBlob(path);
            Console.WriteLine(sha1Hash);

            if (write)
            {
                ObjectStore.WriteObject(sha1Hash, fullBlob);
            }
        }
    }
}
