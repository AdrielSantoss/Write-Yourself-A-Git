using Csharp.Core;
using Git.Core;

namespace Csharp.Commands
{
    public class HashObject
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-w")
            {
                Console.WriteLine("Uso: dotnet run -- hash-object [-w] <arquivo>");
                return string.Empty;
            }

            var write = args[0] == "-w";
            var path = write ? args[1] : args[0];

            if (!File.Exists(path))
            {
                throw new Exception($"Arquivo não encontrado: {path}");
            }

            var (sha1Hash, fullBlob) = BlobUtils.WriteBlob(path);
            Console.WriteLine(sha1Hash);

            if (write)
            {
                ObjectStore.WriteObject(sha1Hash, fullBlob);
            }

            return sha1Hash;
        }
    }
}
