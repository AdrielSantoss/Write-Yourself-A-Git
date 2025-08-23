using Csharp.Core;
using Git.Core;

namespace Csharp.Commands
{
    public class HashObject
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr hash-object [-w] <arquivo>");
                return string.Empty;
            }

            var write = args[0] == "-w";
            var path = write ? args[1] : args[0];

            if (!File.Exists(path))
            {
                throw new Exception($"Arquivo não encontrado: {path}");
            }

            var (sha1Hash, fullBlob) = BlobUtils.WriteBlob(path);

            if (write)
            {
                ObjectStore.WriteObject(sha1Hash, fullBlob);
            }
            else
            {
                Console.WriteLine(sha1Hash);
            }

            return sha1Hash;
        }
    }
}