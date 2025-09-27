using Git.Core;
using System.Runtime.InteropServices;

namespace Git.Commands
{
    public class Diff
    {
        public static string? Execute(string[] args)
        {
            return null;
        }

        [DllImport("diff_tool.dll")]
        public static extern IntPtr myers_diff_c(
            IntPtr[] content_a,
            int len_a,
            IntPtr[] content_b,
            int len_b,
            out int out_len
        );
    }
}
