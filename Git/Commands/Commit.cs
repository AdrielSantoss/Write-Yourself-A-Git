using Csharp.Core;
using Git.Core;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using static System.Net.WebRequestMethods;

namespace Git.Commands
{
    public class Commit
    {
        public static string Execute(string[] args)
        {
            if (args.Length < 2 || args[0] != "-m")
            {
                Console.WriteLine("Uso: dotnet run -- commit [-m] <mensagem_do_commit>");
                return string.Empty;
            }

            var lines = CommitUtils.GetIndexEntries();

            if (!lines.Any())
            {
                throw new Exception("Nenhum arquivo na staging area.");
            }

            var dirEntries = new Dictionary<string, List<TreeEntry>>();

            foreach (var line in lines)
            {
                var parts = line.Split(' ', 2);
                var fileSha1 = parts[0];
                var fullPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), parts[1]);

                var pathArr = fullPath.Split(@"\").ToArray();

                var file = pathArr.Reverse().ToArray()[0];

                var directories = pathArr.Take(pathArr.Length - 1).ToArray();
                var entryKey = file;

                if (directories.Length > 0)
                {
                    entryKey = string.Join(@"\", directories);
                }

                if (!dirEntries.ContainsKey(entryKey))
                {
                    dirEntries[entryKey] = new List<TreeEntry>();
                }

                dirEntries[entryKey].Add(new TreeEntry()
                {
                    Mode = "100644",
                    Name = file,
                    Sha1 = fileSha1
                });
            }

            var mainTries = new Dictionary<string, string>();
            foreach (var key in dirEntries.Keys)
            {
                mainTries[key] = TreeObject.WriteTree(dirEntries[key]);
            }

            string BuildTree(string dirTree)
            {
                var rootTreeEntries = new List<TreeEntry>() { };

                foreach (var key in mainTries.Keys) //a/b/c
                {
                    var subdirs = key.Split(@"\").Reverse().ToArray(); //c, b
                    subdirs = subdirs.Take(subdirs.Length - 1).ToArray();
                    var currentSubDirSha1 = mainTries[key]; //abc123
                    var currentSubdir = key; //a/b/c/maurinho.txt

                    if (subdirs.Length > 1)
                    {
                        foreach (var subdir in subdirs) // c
                        {
                            currentSubdir = subdir; //c
                            currentSubDirSha1 = TreeObject.WriteTree(
                                new List<TreeEntry>()
                                {
                                new TreeEntry()
                                {
                                    Mode = "040000",
                                    Name = currentSubdir, //c 
                                    Sha1 = currentSubDirSha1 //abc123
                                }
                                }
                            );
                        }

                        rootTreeEntries.Add(new TreeEntry()
                        {
                            Mode = "040000",
                            Name = currentSubdir,
                            Sha1 = currentSubDirSha1
                        });
                    }
                    else
                    {
                        rootTreeEntries.Add(new TreeEntry()
                        {
                            Mode = "100644",
                            Name = currentSubdir, 
                            Sha1 = currentSubDirSha1 
                        });
                    }
                }

                return TreeObject.WriteTree(rootTreeEntries);
            }

            var rootSha1 = BuildTree("");

            var commitSha1 = CommitObject.WriteCommit(rootSha1, args[1]);

            //var commitData = Sha1Utils.GetObjectDataBySha1(commitSha1);
            //var nullIndex = Array.IndexOf(commitData, (byte)0);
            //var commitContent = Encoding.UTF8.GetString(commitData[(nullIndex + 1)..]);

            //var commitLines = commitContent.Split('\n');

            //var commitTreeLine = commitLines.Where(commit => commit.StartsWith("tree ")).FirstOrDefault();

            //var commitTreeParts = commitTreeLine.Split(" ", 2);
   
            //var commitTreeSha1 = commitTreeParts[1];

   //         LsTree.Execute(["-p", commitTreeSha1]);

            UpdateHead(commitSha1);

            Console.WriteLine(commitSha1);

            CommitUtils.CreateOrUpdateIndex(string.Empty);

            return commitSha1;
        }

        private static void UpdateHead(string commitSha1)
        {
            var refs = BranchUtils.GetHead();
            var parts = refs.Split(" ", 2);

            BranchUtils.CreateOrUpdateBranch(parts[1], commitSha1);
        }
    }
}
