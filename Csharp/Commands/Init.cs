using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Commands
{
    public class Init
    {
        public static void Execute()
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            if (Directory.Exists(gitDir))
            {
                Console.WriteLine("Repositório já existe.");
                return;
            }

            Directory.CreateDirectory(gitDir);
            Directory.CreateDirectory(Path.Combine(gitDir, "objects"));
            Directory.CreateDirectory(Path.Combine(gitDir, "refs"));

            File.WriteAllText(Path.Combine(gitDir, "HEAD"), "ref: refs/heads/master\n");

            Console.WriteLine($"Repositório inicializado em {gitDir}");
        }
    }
}
