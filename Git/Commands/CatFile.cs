using Csharp.Core;
using System.Text;

namespace Csharp.Commands
{
    public class CatFile
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-p")
            {
                Console.WriteLine("Uso: dotnet run -- cat-file [-p] <hash>");
                return string.Empty;
            }

            var data = ObjectStore.ReadObject(args[1]);
            var nullIndex = Array.IndexOf(data, (byte)0);
            var content = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

            Console.WriteLine(content);
            return content;
        }
    }
}
