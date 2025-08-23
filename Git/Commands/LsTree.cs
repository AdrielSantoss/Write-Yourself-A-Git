using Git.Core;

namespace Git.Commands
{
    public class LsTree
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-p")
            {
                Console.WriteLine("Uso: gitadr ls-tree [-p] <hash>");
                return string.Empty;
            }

            var entries = TreeUtils.GetTreeData(args[1]);

            var result = string.Empty;

            foreach (var entry in entries)
            {
                var type = entry.Mode == "040000" ? "tree" : "blob";
                var lineData = $"{entry.Mode} {type} {entry.Sha1} {entry.Name}";

                Console.WriteLine(lineData);
                result += lineData + "\n";
            }

            return result;
        }
    }
}