using Csharp.Core;
using System.Text;

namespace Csharp.Commands
{
    public class Log
    {
        public static List<string> Execute()
        {
            var lastCommitSha1 = Utils.ReadLastCommitSha1();
            return ReadCommitsRecursive(lastCommitSha1, true, new List<string>());
        }

        public static List<string> ReadCommitsRecursive(string commitSha1, bool isHead, List<string> result)
        {
            var data = Utils.GetObjectDataBySha1(commitSha1);
            var nullIndex = Array.IndexOf(data, (byte)0);
            var content = Encoding.UTF8.GetString(data[(nullIndex + 1)..]);

            var lines = content.Split('\n');

            var parent = string.Empty;
            var author = string.Empty;
            var committer = string.Empty;
            int messageIndex = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("parent "))
                    parent = line.Substring(7);
                else if (line.StartsWith("author "))
                    author = line.Substring(7);
                else if (line.StartsWith("committer "))
                    committer = line.Substring(10);
                else if (string.IsNullOrWhiteSpace(line))
                {
                    messageIndex = i + 1;
                    break;
                }
            }

            var message = messageIndex >= 0 && messageIndex < lines.Length ? string.Join('\n', lines[messageIndex..]) : string.Empty;

            var authorNameEmail = author;
            var dateString = string.Empty;

            if (author != null)
            {
                var parts = author.Split(' ');
                if (parts.Length >= 3)
                {
                    string timestampStr = parts[^2];
                    string timezone = parts[^1];

                    if (long.TryParse(timestampStr, out long unixSeconds))
                    {
                        var dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
                        dateString = dateTimeUtc.ToString("yyyy-MM-dd HH:mm:ss") + " " + timezone;

                        authorNameEmail = string.Join(' ', parts[..^2]);
                    }
                }
            }

            var output = $@"
commit {commitSha1}{(isHead ? " (HEAD -> master)" : string.Empty)}
Author: {authorNameEmail}
Date: {dateString}

    {message.Replace("\n", "\n    ")}
";

            Console.WriteLine(output);

            result.Add(output);

            if (!string.IsNullOrEmpty(parent))
            {
                return ReadCommitsRecursive(parent, false, result);
            } 
            else
            {
                return result;
            }
        }
    }
}
