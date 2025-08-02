using Csharp.Core;
using System.Collections.Generic;

namespace Csharp.Commands
{
    public class WriteTree
    {

        private static string[] ignoreFiles = { ".gitadr", "Program.cs", "Git.csproj", "bin", "Commands", "Core", "obj", ".vs" };

        public static void Execute()
        {
            var entries = new List<TreeEntry>();

            ExecuteDirectory(new List<string>() { Directory.GetCurrentDirectory() });

            //CreateBlobObject(files, entries);

            ////foreach (var file in files)
            ////{
            ////    entries.Add(new TreeEntry
            ////    {
            ////        Mode = "100644",
            ////        Name = Path.GetFileName(file),
            ////        Sha1 = blobSha1
            ////    });
            ////}
            
            //string treeSha1 = TreeObject.WriteTree(entries);
            //Console.WriteLine(treeSha1);
        }

        public static List<string> GetAllFiles(string currentDirectory)
            => Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), currentDirectory)).Where(f => !ignoreFiles.Where(i => f.Contains(i)).Any()).ToList();

        public static List<string> GetAllDirectories(string currentDirectory)
            => Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), currentDirectory)).Where(d => !ignoreFiles.Where(i => d.Contains(i)).Any()).ToList();

        public static void CreateBlobObject(List<string> files)
        {
            foreach (var file in files)
            {
                if (ignoreFiles.Any(ignore => file.Contains(ignore)))
                {
                    continue;
                }

                var (blobSha1, fullBlob) = Utils.WriteBlob(file);
                ObjectStore.WriteObject(blobSha1, fullBlob);
            }
        }

        public static void CreateTreeObject(List<string> files)
        {
            foreach (var file in files)
            {
                if (ignoreFiles.Any(ignore => file.Contains(ignore)))
                {
                    continue;
                }

                var (blobSha1, fullBlob) = Utils.WriteBlob(file);
                ObjectStore.WriteObject(blobSha1, fullBlob);
            }
        }

        public static void ExecuteDirectory(List<string> currentDirectories)
        {
            if (currentDirectories.Count > 0) 
            {
                Console.WriteLine(currentDirectories[0]);
                var currentDirectory = currentDirectories[0];
                var files = GetAllFiles(currentDirectory);

                CreateBlobObject(files);

                currentDirectories = currentDirectories.Where(c => c != currentDirectory).ToList();

                currentDirectories.AddRange(GetAllDirectories(currentDirectory));

                Console.WriteLine(currentDirectories.Count);

                ExecuteDirectory(currentDirectories);
            }
        }
    }
}
