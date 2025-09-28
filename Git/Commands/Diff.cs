using Git.Core;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Git.Commands
{
    public class Diff
    {
        public static string? Execute(string[] args)
        {
            var contentA = new[] { "linha1", "linha2" };
            var contentB = new[] { "linha1", "linhaX", "linha2" };

            return RunDiff(contentA, contentB);
        }

        private static string RunDiff(string[] contentA, string[] contentB)
        {
            IntPtr[] aPtrs = contentA.Select(Marshal.StringToHGlobalAnsi).ToArray();
            IntPtr[] bPtrs = contentB.Select(Marshal.StringToHGlobalAnsi).ToArray();

            try
            {
                int outLen;
                IntPtr resultPtr = myers_diff_c(aPtrs, aPtrs.Length, bPtrs, bPtrs.Length, out outLen);

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
        private static extern IntPtr myers_diff_c(
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
