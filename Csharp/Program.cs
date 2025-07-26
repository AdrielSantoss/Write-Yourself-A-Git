using Csharp.Commands;

if (args.Length == 0)
{
    Console.WriteLine("Uso: dotnet run -- <comando_git>");
    return;
}


switch (args[0])
{
    case "init":
        GitInit.Execute();
        break;

    default:
        Console.WriteLine($"Comando desconhecido: {args[0]}");
        break;
}