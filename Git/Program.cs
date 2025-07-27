using Csharp.Commands;

if (args.Length == 0)
{
    Console.WriteLine("Uso: dotnet run -- <comando_git>");
    return;
}

switch (args[0])
{
    case "init":
        Init.Execute();
        break;

    case "hash-object":
        HashObject.Execute(args[1..]);
        break;

    case "cat-file":
        CatFile.Execute(args[1..]);
        break;

    default:
        Console.WriteLine($"Comando desconhecido: {args[0]}");
        break;
}