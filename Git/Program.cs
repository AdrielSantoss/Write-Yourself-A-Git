using Csharp.Commands;
using Git.Commands;

if (args.Length == 0)
{
    Console.WriteLine("Uso: dotnet run -- <comando_git>");
    return;
}

if (args[0] != "init" && !Directory.Exists(".gitadr"))
{
    Console.WriteLine("GitAdr não inicializado.");
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

    case "write-tree":
        WriteTree.Execute();
        break;

    case "ls-tree":
        LsTree.Execute(args[1..]);
        break;

    case "commit":
        Commit.Execute(args[1..]);
        break;

    case "log":
        Log.Execute();
        break;

    default:
        Console.WriteLine($"Comando desconhecido: {args[0]}");
        break;
}