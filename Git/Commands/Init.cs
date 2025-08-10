namespace Csharp.Commands
{
    public class Init
    {
        public static void Execute()
        {
            var gitDir = Path.Combine(Directory.GetCurrentDirectory(), ".gitadr");

            if (Directory.Exists(gitDir))
            {
                Console.WriteLine("Repositório .gitadr já existe.");
                return;
            }

            Directory.CreateDirectory(gitDir);
            Directory.CreateDirectory(Path.Combine(gitDir, "objects"));
            Directory.CreateDirectory(Path.Combine(gitDir, "refs"));
            Directory.CreateDirectory(Path.Combine(gitDir, "refs", "heads"));

            File.Create(Path.Combine(gitDir, "refs", "heads", "master"));
            File.WriteAllText(Path.Combine(gitDir, "HEAD"), "ref: refs/heads/master\n");

            Console.WriteLine($"Repositório inicializado em {gitDir}");
        }
    }
}
