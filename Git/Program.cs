using Csharp.Commands;
using Git.Commands;

if (args.Length == 0)
{
    Console.WriteLine("Uso: gitadr <comando_git>");
    return;
}

if (args[0] != "init" && !ReadCurrentWorkingDirectory(Directory.GetCurrentDirectory()))
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

    case "add":
        Add.Execute(args[1..]);
        break;

    case "rm":
        Rm.Execute(args[1..]);
        break;

    case "restore":
        Restore.Execute(args[1..]);
        break;

    case "commit":
        Commit.Execute(args[1..]);
        break;

    case "log":
        Log.Execute();
        break;

    case "branch":
        Branch.Execute(args[1..]);
        break;

    case "switch":
        Switch.Execute(args[1..]);
        break;

    case "status":
        Status.Execute();
        break;

    case "merge":
        Merge.Execute(args[1..]);
        break;

    case "diff":
        Diff.Execute(args[1..]);
        break;

    default:
        Console.WriteLine($"Comando desconhecido: {args[0]}");
        break;
}

static bool ReadCurrentWorkingDirectory(string directory)
{
    if (Directory.Exists(directory))
    {
        if (Directory.Exists(Path.Combine(directory, ".gitadr"))) 
        {
            Directory.SetCurrentDirectory(directory);
            return true;
        }

        var dirs = directory.Split(Path.DirectorySeparatorChar);
        var taked = dirs.Take(dirs.Length - 1).ToArray();

        if (taked.Length > 0)
        {
            return ReadCurrentWorkingDirectory(string.Join(Path.DirectorySeparatorChar, taked));
        }
    }

    return false;
}