using Csharp.Commands;
using Git.Core;
using System;

namespace Git.Commands
{
    public class Add
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr add <arquivo | diretório | .>");
                return;
            }

            var target = args[0];

            if (target == ".")
            {
                AddAll();
                return;
            }

            if (Directory.Exists(target))
            {
                ExecuteRecursive(Path.GetFullPath(target));
                StageDeletionsUnderPath(Path.GetFullPath(target));
                return;
            }

            var rel = Path.GetRelativePath(Directory.GetCurrentDirectory(), target);
            AddOrUpdateIndexFile(rel);
        }

        public static void AddAll()
        {
            var workspaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());
            var indexFiles = IndexUtils.GetIndexEntries(true);

            foreach (var file in workspaceFiles.Keys)
            {
                AddOrUpdateIndexFile(file);
            }

            foreach (var file in indexFiles.Keys.ToList())
            {
                if (!workspaceFiles.ContainsKey(file))
                {
                    indexFiles.Remove(file);
                    IndexUtils.CreateOrUpdateIndex(string.Join('\n', indexFiles.Select(kv => $"{kv.Value} {kv.Key}")) + "\n");
                }
            }
        }

        public static void ExecuteRecursive(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(file) == ignoreFile))
                    continue;

                AddOrUpdateIndexFile(Path.GetRelativePath(Directory.GetCurrentDirectory(), file));
            }

            foreach (var subdir in Directory.GetDirectories(directory))
            {
                if (IndexUtils.ignoreFiles.Any(ignoreFile => Path.GetFileName(subdir) == ignoreFile))
                    continue;

                ExecuteRecursive(subdir);
            }
        }

        private static void StageDeletionsUnderPath(string basePath)
        {
            var root = Directory.GetCurrentDirectory();
            var relBase = Path.GetRelativePath(root, basePath);
            if (relBase == "." || string.IsNullOrEmpty(relBase))
            {
                relBase = string.Empty;
            }

            var index = IndexUtils.GetIndexEntries(true);
            var newContentLines = new List<string>();

            foreach (var kv in index)
            {
                var file = kv.Key;
                var sha1 = kv.Value;

                bool underBase =
                    string.IsNullOrEmpty(relBase) ||
                    file == relBase ||
                    file.StartsWith(relBase + Path.DirectorySeparatorChar);

                if (underBase)
                {
                    var fullPath = Path.Combine(root, file);
                    if (!File.Exists(fullPath))
                    {
                        continue;
                    }
                }

                newContentLines.Add($"{sha1} {file}");
            }

            IndexUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }

        public static void AddOrUpdateIndexFile(string file, string? sha1Param = null)
        {
            var index = IndexUtils.GetIndexEntries(true);

            if (!File.Exists(file))
            {
                if (index.ContainsKey(file))
                {
                    index.Remove(file);
                    IndexUtils.CreateOrUpdateIndex(string.Join('\n', index.Select(kv => $"{kv.Value} {kv.Key}")) + "\n");
                }
                return;
            }

            var sha1 = sha1Param ?? HashObject.Execute(new string[] { "-w", file });
            index[file] = sha1;

            IndexUtils.CreateOrUpdateIndex(string.Join('\n', index.Select(kv => $"{kv.Value} {kv.Key}")) + "\n");
        }
    }
}
