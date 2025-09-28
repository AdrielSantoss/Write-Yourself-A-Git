using Git.Core;
using System.Runtime.InteropServices;
using System.Text;

namespace Git.Commands
{
    public class Diff
    {
        public static void Execute(string[] args)
        {
            // gitadr diff → working directory vs index
            var workspaceFiles = IndexUtils.RecursiveReadWorkSapce(Directory.GetCurrentDirectory(), new Dictionary<string, string>());
            var indexFiles = IndexUtils.GetIndexEntries(false);

            var allFiles = workspaceFiles.Keys
            .Union(indexFiles.Keys)
            .Union(workspaceFiles.Keys);

            foreach (var file in allFiles)
            {
                if (indexFiles.ContainsKey(file))
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

                    RunDiff(contentA.Split("\n"), contentB.Split("\n"));
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

        private static string RunDiff(string[] contentA, string[] contentB)
        {
            IntPtr[] aPtrs = contentA.Select(Marshal.StringToHGlobalAnsi).ToArray();
            IntPtr[] bPtrs = contentB.Select(Marshal.StringToHGlobalAnsi).ToArray();

            try
            {
                int outLen;
                IntPtr resultPtr = patience_diff(aPtrs, aPtrs.Length, bPtrs, bPtrs.Length, out outLen);

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
