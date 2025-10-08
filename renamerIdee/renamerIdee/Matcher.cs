using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace renamerIdee
{
    class Matcher
    {
        static string VERSION = "V2.7";

        public static void RenameFilesInFolder(string folderPath, string oldName, string newName)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("❌ Folder does not exist!");
                return;
            }

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

            if (oldName.Contains("*") &&
                !newName.Contains("*") &&
                !newName.Contains("[a]") &&
                !newName.Contains("[A]"))
            {
                int startNum = ExtractFirstNumber(newName) ?? 1;
                int counter = startNum;
                var updated = new List<(string oldPath, string newPath)>();

                foreach (var (oldPath, _) in matchedFiles)
                {
                    string dir = Path.GetDirectoryName(oldPath);
                    string ext = Path.GetExtension(oldPath);
                    string baseName = Path.GetFileNameWithoutExtension(newName);

                    if (Regex.IsMatch(baseName, @"\d+"))
                        baseName = Regex.Replace(baseName, @"\d+", counter.ToString());
                    else
                        baseName = $"{baseName}{counter}";

                    string newFileName = $"{baseName}{ext}";
                    updated.Add((oldPath, Path.Combine(dir, newFileName)));
                    counter++;
                }

                matchedFiles = updated;
            }

            for (int i = 0; i < matchedFiles.Count; i++)
            {
                string oldFile = Path.GetFileNameWithoutExtension(matchedFiles[i].oldPath);
                string newFile = Path.GetFileNameWithoutExtension(matchedFiles[i].newPath);
                string ext = Path.GetExtension(matchedFiles[i].oldPath);
                string dir = Path.GetDirectoryName(matchedFiles[i].oldPath);

                bool lowercasePattern = newName.Contains("[a]");
                bool uppercasePattern = newName.Contains("[A]");

                if (lowercasePattern || uppercasePattern)
                {
                    var numMatch = Regex.Match(oldFile, @"(\d+)");
                    if (numMatch.Success)
                    {
                        int num = int.Parse(numMatch.Groups[1].Value);
                        if (num >= 1 && num <= 26)
                        {
                            char letter = (char)((lowercasePattern ? 'a' : 'A') + num - 1);
                            string baseName = newFile
                                .Replace("[a]", letter.ToString())
                                .Replace("[A]", letter.ToString());

                            string newPath = Path.Combine(dir, baseName + ext);
                            matchedFiles[i] = (matchedFiles[i].oldPath, newPath);
                        }
                    }
                }
            }

            if (matchedFiles.Count == 0)
            {
                Console.WriteLine("⚠️  No files matched the pattern.");
                Console.WriteLine($"   (regex used: {regexPattern})");
                Console.WriteLine("   Existing files:");
                foreach (var f in files)
                    Console.WriteLine("     - " + Path.GetFileName(f));
                return;
            }

            Console.WriteLine("\nPreview of changes:");
            foreach (var (oldPath, newPath) in matchedFiles)
                Console.WriteLine($"  {Path.GetFileName(oldPath)}  →  {Path.GetFileName(newPath)}");

            Console.Write("\nApply these changes? (Y/N): ");
            string answer = Console.ReadLine().Trim().ToUpper();

            if (answer == "Y")
            {
                foreach (var (oldPath, newPath) in matchedFiles)
                {
                    string tempPath = newPath + ".tmp";
                    File.Move(oldPath, tempPath);
                }

                foreach (var (oldPath, newPath) in matchedFiles)
                {
                    string tempPath = newPath + ".tmp";
                    File.Move(tempPath, newPath);
                    Console.WriteLine($"✅ {Path.GetFileName(oldPath)} → {Path.GetFileName(newPath)}");
                }

                Console.WriteLine("\n🎉 All files renamed successfully!");
            }
            else
            {
                Console.WriteLine("❌ Operation cancelled.");
            }
        }

        static int? ExtractFirstNumber(string input)
        {
            var m = Regex.Match(input, @"\d+");
            if (m.Success && int.TryParse(m.Value, out int val))
                return val;
            return null;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine($"     Ultimate File Renamer (version {VERSION})");
            Console.WriteLine("==============================================\n");

            string currentFolder = null;

            while (true)
            {
                if (string.IsNullOrEmpty(currentFolder))
                {
                    Console.Write("Enter folder path containing files (or 'exit' to quit):\n>> ");
                    string folder = Console.ReadLine();

                    if (folder.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (!Directory.Exists(folder))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Folder does not exist. Try again.\n");
                        Console.ResetColor();
                        continue;
                    }

                    currentFolder = folder;
                }

                var files = Directory.GetFiles(currentFolder);
                Console.WriteLine($"\n📂 Found {files.Length} file(s):");
                foreach (var f in files)
                    Console.WriteLine("   - " + Path.GetFileName(f));

                Console.Write("\n🔤 OLD filename pattern (use * as wildcard):\n>> ");
                string oldPattern = Console.ReadLine();

                Console.Write("🆕 NEW filename pattern (use * as wildcard, or [a]/[A] for ASCII letters):\n>> ");
                string newPattern = Console.ReadLine();

                RenameFilesInFolder(currentFolder, oldPattern, newPattern);

                Console.WriteLine("\nOptions: (R)ename again | (C)hange folder | (E)xit");
                Console.Write(">> ");
                string option = Console.ReadLine().Trim().ToUpper();

                if (option == "E") break;
                if (option == "C")
                {
                    currentFolder = null;
                    continue;
                }
                // If R → keep currentFolder and repeat
            }

            Console.WriteLine("\n👋 Goodbye!");
        }
    }
}
