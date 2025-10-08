using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace renamerIdee
{
    public static class PatternMatcher
    {
        public static List<(string oldPath, string newPath)> MatchFiles(string folderPath, string oldName, string newName)
        {
            var files = Directory.GetFiles(folderPath).OrderBy(f => f).ToList();

            string escaped = Regex.Escape(oldName);
            string regexPattern = escaped.Replace("\\*", "(.*?)");

            if (!oldName.StartsWith("*"))
                regexPattern = "^" + regexPattern;
            if (!oldName.EndsWith("*"))
                regexPattern = regexPattern + "$";

            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            int starIndex = 1;
            string replacementPattern = Regex.Replace(newName, "\\*", m => $"{{{starIndex++}}}");

            var matchedFiles = new List<(string oldPath, string newPath)>();
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string dir = Path.GetDirectoryName(file);

                Match match = regex.Match(fileName);
                if (match.Success)
                {
                    string newFileName = replacementPattern;
                    for (int g = 1; g < match.Groups.Count; g++)
                        newFileName = newFileName.Replace("{" + g + "}", match.Groups[g].Value);

                    matchedFiles.Add((file, Path.Combine(dir, newFileName)));
                }
            }
            return matchedFiles;
        }
    }
}
