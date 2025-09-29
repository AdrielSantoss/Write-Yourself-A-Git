using Git.Core;
using System.Runtime.InteropServices;
using System.Text;

namespace Git.Commands
{
    public class Diff
    {
        public static void Execute(string[] args)
        {
            var isPatience = args.Any(arg => arg == "--patience");

            if (
                args.Length == 0 || 
                args.Length == 1 && isPatience
            ) {
                DiffIndexWorkSpace(isPatience);
                return;
            }

            if (args[0] == "--staged")
            {
                DiffHeadIndex(isPatience);
                return;
            }

            if (args[0] == "HEAD")
            {
                DiffHeadWorkspace(isPatience);
                return;
            }

            if (
                args.Length == 2 ||
                args.Length == 3 && isPatience
            )
            {
                DiffTwoCommits(args[0], args[1], isPatience);
                return;
            }
        }

        private static void DiffTwoCommits(string commit1sha1, string commit2sha1, bool isPatience)
        {
            var commit1TreeSha1 = CommitUtils.GetCommitTreeSha1(commit1sha1);
            var commit1Files = TreeUtils.GetTreeEntriesFromSha1("", commit1TreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var commit2TreeSha1 = CommitUtils.GetCommitTreeSha1(commit2sha1);
            var commit2Files = TreeUtils.GetTreeEntriesFromSha1("", commit2TreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var allFiles = commit1Files.Keys.Union(commit2Files.Keys);

            foreach (var file in allFiles)
            {
                if (
                    commit1Files.ContainsKey(file) &&
                    commit2Files.ContainsKey(file) &&
                    commit1Files[file].Sha1 != commit2Files[file].Sha1
                )
                {
                    var sha1A = commit1Files[file].Sha1;
                    var sha1B = commit2Files[file].Sha1;

                    ShowDiffHead(file, file, sha1A, sha1B);

                    var data = Sha1Utils.GetObjectDataBySha1(sha1A);
                    var nullIndex = Array.IndexOf(data, (byte)0);
                    var contentA = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

                    var dataB = Sha1Utils.GetObjectDataBySha1(sha1B);
                    var nullIndexB = Array.IndexOf(dataB, (byte)0);
                    var contentB = Encoding.UTF8.GetString(dataB[(nullIndexB + 1)..]);

                    if (string.IsNullOrWhiteSpace(contentA) && string.IsNullOrWhiteSpace(contentB))
                    {
                        return;
                    }

                    RunDiff(contentA.Split("\n"), contentB.Split("\n"), isPatience);
                }
            }
        }

        private static void DiffHeadWorkspace(bool isPatience)
        {
            var commitSha1 = CommitUtils.GetLastCommitSha1FromHead();
            var treeSha1 = CommitUtils.GetCommitTreeSha1(commitSha1);
            var headFiles = TreeUtils.GetTreeEntriesFromSha1("", treeSha1, new Dictionary<string, (string Mode, string Sha1)>());
            var workspaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());

            var allFiles = workspaceFiles.Keys.Union(headFiles.Keys);

            foreach (var file in allFiles)
            {
                if (
                    workspaceFiles.ContainsKey(file) &&
                    headFiles.ContainsKey(file) &&
                    workspaceFiles[file] != headFiles[file].Sha1
                )
                {
                    var sha1A = headFiles[file].Sha1;
                    var sha1B = workspaceFiles[file];

                    ShowDiffHead(file, file, sha1A, sha1B);

                    var data = Sha1Utils.GetObjectDataBySha1(sha1A);
                    var nullIndex = Array.IndexOf(data, (byte)0);
                    var contentA = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

                    var contentB = File.ReadAllText(file);

                    if (string.IsNullOrWhiteSpace(contentA) && string.IsNullOrWhiteSpace(contentB))
                    {
                        return;
                    }

                    RunDiff(contentA.Split("\n"), contentB.Split("\n"), isPatience);
                }
            }
        }

        private static void DiffHeadIndex(bool isPatience)
        {
            var commitSha1 = CommitUtils.GetLastCommitSha1FromHead();
            var treeSha1 = CommitUtils.GetCommitTreeSha1(commitSha1);
            var headFiles = TreeUtils.GetTreeEntriesFromSha1("", treeSha1, new Dictionary<string, (string Mode, string Sha1)>());
            var indexFiles = IndexUtils.GetIndexEntries(false);

            var allFiles = indexFiles.Keys.Union(headFiles.Keys);

            foreach (var file in allFiles)
            {
                if (
                    indexFiles.ContainsKey(file) && 
                    headFiles.ContainsKey(file) &&
                    indexFiles[file] != headFiles[file].Sha1
                )
                {
                    var sha1A = headFiles[file].Sha1;
                    var sha1B = indexFiles[file];

                    ShowDiffHead(file, file, sha1A, sha1B);

                    var data = Sha1Utils.GetObjectDataBySha1(sha1A);
                    var nullIndex = Array.IndexOf(data, (byte)0);
                    var contentA = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

                    var dataB = Sha1Utils.GetObjectDataBySha1(sha1B);
                    var nullIndexB = Array.IndexOf(dataB, (byte)0);
                    var contentB = Encoding.UTF8.GetString(dataB[(nullIndexB + 1)..]);

                    if (string.IsNullOrWhiteSpace(contentA) && string.IsNullOrWhiteSpace(contentB))
                    {
                        return;
                    }

                    RunDiff(contentA.Split("\n"), contentB.Split("\n"), isPatience);
                }
            }
        }

        private static void DiffIndexWorkSpace(bool isPatience)
        {
            var workspaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());
            var indexFiles = IndexUtils.GetIndexEntries(false);

            var allFiles = workspaceFiles.Keys.Union(indexFiles.Keys);

            foreach (var file in allFiles)
            {
                if (
                    indexFiles.ContainsKey(file) &&
                    workspaceFiles.ContainsKey(file) &&
                    indexFiles[file] != workspaceFiles[file]
                )
                {
                    var sha1A = indexFiles[file];
                    var sha1B = Sha1Utils.CreateSha1FromByteData(File.ReadAllBytes(file));

                    ShowDiffHead(file, file, sha1A, sha1B);

                    var data = Sha1Utils.GetObjectDataBySha1(sha1A);
                    var nullIndex = Array.IndexOf(data, (byte)0);
                    var contentA = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

                    var contentB = File.ReadAllText(file);

                    if (string.IsNullOrWhiteSpace(contentA) && string.IsNullOrWhiteSpace(contentB))
                    {
                        return;
                    }

                    RunDiff(contentA.Split("\n"), contentB.Split("\n"), isPatience);
                }
            }
        }

        private static void ShowDiffHead(string fileA, string fileB, string sha1A, string sha1B)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"diff --gitadr a{Path.DirectorySeparatorChar}{Path.GetFileName(fileA)} b{Path.DirectorySeparatorChar}{Path.GetFileName(fileB)}");
            Console.WriteLine($"index {sha1A.Substring(0, 7)}..{sha1B.Substring(0, 7)} 100644");
            Console.WriteLine($"--- a{Path.DirectorySeparatorChar}{Path.GetFileName(fileA)}");
            Console.WriteLine($"+++ b{Path.DirectorySeparatorChar}{Path.GetFileName(fileB)}");
            Console.ResetColor();
        }

        private static string RunDiff(string[] contentA, string[] contentB, bool isPatience)
        {
            IntPtr[] aPtrs = contentA.Select(Marshal.StringToHGlobalAnsi).ToArray();
            IntPtr[] bPtrs = contentB.Select(Marshal.StringToHGlobalAnsi).ToArray();

            try
            {
                int outLen;
                IntPtr resultPtr = isPatience ? 
                    patience_diff(aPtrs, aPtrs.Length, bPtrs, bPtrs.Length, out outLen) : 
                    myers_diff(aPtrs, aPtrs.Length, bPtrs, bPtrs.Length, out outLen);

                if (resultPtr == IntPtr.Zero || outLen == 0)
                    return string.Empty;

                IntPtr[] results = new IntPtr[outLen];
                Marshal.Copy(resultPtr, results, 0, outLen);

                string[] diffLines = results.Select(ptr => Marshal.PtrToStringAnsi(ptr) ?? "").ToArray();

                free_diff(resultPtr, outLen);

                foreach (var line in diffLines)
                {
                    if (line.StartsWith("+"))
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (line.StartsWith("-"))
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(line);
                }
                Console.ResetColor();

                return string.Join(Environment.NewLine, diffLines);
            }
            finally
            {
                foreach (var p in aPtrs) Marshal.FreeHGlobal(p);
                foreach (var p in bPtrs) Marshal.FreeHGlobal(p);
            }
        }

        [DllImport("diff_tool.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr myers_diff(
            IntPtr[] content_a,
            int len_a,
            IntPtr[] content_b,
            int len_b,
            out int out_len
        );

        [DllImport("diff_tool.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr patience_diff(
            IntPtr[] content_a,
            int len_a,
            IntPtr[] content_b,
            int len_b,
            out int out_len
        );

        [DllImport("diff_tool.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void free_diff(IntPtr result, int len);
    }
}
