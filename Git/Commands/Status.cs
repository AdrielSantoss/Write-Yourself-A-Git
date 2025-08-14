using Csharp.Core;

namespace Git.Commands
{
    public class Status
    {
        public static void Execute()
        {
            Console.WriteLine(Utils.GetHeadFileContent());
        }
    }
}
