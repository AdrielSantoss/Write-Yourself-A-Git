using Git.Core;

namespace Git.Commands
{
    public class Merge
    {
        public static void Execute(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: gitadr merge <branch>");
                return;
            }

            var targetBranch = args[0];

            var headTargetBranch = BranchUtils.GetCommitHeadFromBranch(targetBranch);

            if (headTargetBranch == null)
            {
                Console.WriteLine($"Não existe um branch com o nome {targetBranch}");
                return;
            }

            var headCommit = CommitUtils.GetLastCommitSha1FromHead();
            var headBranch = BranchUtils.GetHead();

            var headTarget = BranchUtils.GetCommitHeadFromBranch(targetBranch);

            var headCommits = BranchUtils.GetAllCommitsFromBranch(headBranch.Replace(@$"ref: refs{Path.DirectorySeparatorChar}heads{Path.DirectorySeparatorChar}", string.Empty));
            var targetCommits = BranchUtils.GetAllCommitsFromBranch(targetBranch);

            if (headCommits!.All(commit => targetCommits!.Contains(commit))) 
            {
                Console.WriteLine($"Already up to date.");

                return;
            }

            var commitBase = targetCommits!.Intersect(headCommits!).First();

            var baseTreeSha1 = CommitUtils.GetCommitTreeSha1(commitBase);
            var baseEntries = TreeUtils.GetTreeEntriesFromSha1(string.Empty, baseTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var headTreeSha1 = CommitUtils.GetCommitTreeSha1(headCommit);
            var headEntries = TreeUtils.GetTreeEntriesFromSha1(string.Empty, headTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var targetTreeSha1 = CommitUtils.GetCommitTreeSha1(headTarget!);
            var targetEntries = TreeUtils.GetTreeEntriesFromSha1(string.Empty, targetTreeSha1, new Dictionary<string, (string Mode, string Sha1)>());

            var allFiles = baseEntries.Keys
                .Union(headEntries.Keys)
                .Union(targetEntries.Keys);

            foreach (var file in allFiles)
            {
                var headSha1 = headEntries.ContainsKey(file) ? headEntries[file].Sha1 : string.Empty;
                var alvoSha1 = targetEntries.ContainsKey(file) ? targetEntries[file].Sha1 : string.Empty;
                var ancestralSha1 = baseEntries.ContainsKey(file) ? baseEntries[file].Sha1 : string.Empty;

                if (!string.IsNullOrWhiteSpace(headSha1) &&
                    !string.IsNullOrWhiteSpace(alvoSha1) &&
                    headSha1 == alvoSha1)
                {
                    continue;
                }

                if ((headSha1 == ancestralSha1) && (alvoSha1 != ancestralSha1))
                {
                    AddOrUpdateIndexFile(file, alvoSha1);
                    Sha1Utils.WriteFileAndDirectoriesFromSha1(Path.Combine(Directory.GetCurrentDirectory(), file), alvoSha1);

                    continue;
                }

                if ((alvoSha1 == ancestralSha1) && (headSha1 != ancestralSha1))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(ancestralSha1) &&
                    (headSha1 != ancestralSha1) &&
                    (alvoSha1 != ancestralSha1) &&
                    (headSha1 != alvoSha1))
                {
                    Console.WriteLine($"Ocorreu um conflito no arquivo: {file}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(ancestralSha1))
                {
                    if (!string.IsNullOrWhiteSpace(headSha1) && string.IsNullOrWhiteSpace(alvoSha1))
                    {
                        continue;
                    }
                    else if (!string.IsNullOrWhiteSpace(alvoSha1) && string.IsNullOrWhiteSpace(headSha1))
                    {
                        AddOrUpdateIndexFile(file, alvoSha1);
                        Sha1Utils.WriteFileAndDirectoriesFromSha1(Path.Combine(Directory.GetCurrentDirectory(), file), alvoSha1);

                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(ancestralSha1))
                {
                    if (string.IsNullOrWhiteSpace(headSha1) && string.IsNullOrWhiteSpace(alvoSha1))
                    {
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(headSha1) && alvoSha1 == ancestralSha1)
                    {
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(alvoSha1) && headSha1 == ancestralSha1)
                    {
                        RemoveIndexFile(file);
                        File.Delete(Path.Combine(Directory.GetCurrentDirectory(), file));
                    }
                }
            }

            BranchUtils.CreateOrUpdateBranch(headBranch.Replace("ref: ", string.Empty), headTarget!);
        }

        public static void AddOrUpdateIndexFile(string file, string sha1)
        {
            var lines = CommitUtils.GetIndexEntries(true);

            var newContentLines = new List<string>();
            var found = false;

            foreach (var fileName in lines.Keys)
            {
                var fileSha1 = lines[fileName];

                if (fileName == file)
                {
                    found = true;
                    if (fileSha1 == sha1)
                    {
                        return;
                    }
                    else
                    {
                        newContentLines.Add($"{sha1} {fileName}");
                    }
                }
                else
                {
                    newContentLines.Add($"{fileSha1} {fileName}");
                }
            }

            if (!found)
            {
                newContentLines.Add($"{sha1} {file}");
            }

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }

        public static void RemoveIndexFile(string file)
        {
            var lines = CommitUtils.GetIndexEntries(true);
            lines.Remove(file);
            var newContentLines = new List<string>();

            foreach (var fileName in lines.Keys)
            {
                newContentLines.Add($"{lines[fileName]} {fileName}");
            }

            CommitUtils.CreateOrUpdateIndex(string.Join('\n', newContentLines) + "\n");
        }
    }
}