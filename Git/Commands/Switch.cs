using Csharp.Core;

namespace Git.Commands
{
    public class Switch
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run -- switch <nome_do_branch>");
                return;
            }

            var branchName = args[0];

            var existHeadFile = Utils.GetBranchFileContent(branchName);

            if (existHeadFile == null)
            {
                Console.WriteLine($"Não existe um branch com o nome {branchName}");
                return;
            }

            Utils.WriteHeadFileContent(@$"ref: refs\heads\{branchName}");

            Console.WriteLine($"Branch atual alterado com sucesso para {branchName}");
        }
    }
}
