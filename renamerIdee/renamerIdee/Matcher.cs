using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace renamerIdee
{
    class Matcher
    {
        static string VERSION = "V2.4";

        public static void RenameFilesInFolder(string folderPath, string oldName, string newName)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("❌ Folder does not exist!");
                return;
            }

            var files = Directory.GetFiles(folderPath).OrderBy(f => f).ToList();

            // --- Convert old pattern to regex (wildcards * → match anything) ---
            string escaped = Regex.Escape(oldName);
            string regexPattern = escaped.Replace("\\*", "(.*?)");

            // Only anchor if pattern clearly defines start or end
            if (!oldName.StartsWith("*"))
                regexPattern = "^" + regexPattern;
            if (!oldName.EndsWith("*"))
                regexPattern = regexPattern + "$";

            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            // Replacement pattern setup
            int starIndex = 1;
            string replacementPattern = Regex.Replace(newName, "\\*", m => $"{{{starIndex++}}}");

            var matchedFiles = new List<(string oldPath, string newPath)>();

            // --- Try to match files ---
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string dir = Path.GetDirectoryName(file);

                Match match = regex.Match(fileName);
                if (match.Success)
                {
                    string newFileName = replacementPattern;

                    // Replace * with captured text
                    for (int g = 1; g < match.Groups.Count; g++)
                        newFileName = newFileName.Replace("{" + g + "}", match.Groups[g].Value);

                    matchedFiles.Add((file, Path.Combine(dir, newFileName)));
                }
            }

            // --- Handle sequential renumbering ---
            if (oldName.Contains("*") && !newName.Contains("*"))
            {
                int startNum = ExtractFirstNumber(newName) ?? 1;
                int counter = startNum;
                var updated = new List<(string oldPath, string newPath)>();

                foreach (var (oldPath, _) in matchedFiles)
                {
                    string dir = Path.GetDirectoryName(oldPath);
                    string ext = Path.GetExtension(oldPath);

                    // Remove any leading numbers or dash from the newName base
                    string baseName = Path.GetFileNameWithoutExtension(newName);
                    baseName = Regex.Replace(baseName, @"^\d+\-?", ""); // e.g. "1-test" -> "test"

                    string newFileName = $"{counter++}-{baseName}{ext}";
                    updated.Add((oldPath, Path.Combine(dir, newFileName)));
                }

                matchedFiles = updated;
            }

            // --- Handle case: no matches ---
            if (matchedFiles.Count == 0)
            {
                Console.WriteLine("⚠️  No files matched the pattern.");
                Console.WriteLine($"   (regex used: {regexPattern})");
                Console.WriteLine("   Existing files:");
                foreach (var f in files)
                    Console.WriteLine("     - " + Path.GetFileName(f));
                return;
            }

            // --- Preview changes ---
            Console.WriteLine("\nPreview of changes:");
            foreach (var (oldPath, newPath) in matchedFiles)
                Console.WriteLine($"  {Path.GetFileName(oldPath)}  →  {Path.GetFileName(newPath)}");

            Console.Write("\nApply these changes? (Y/N): ");
            string answer = Console.ReadLine().Trim().ToUpper();

            if (answer == "Y")
            {
                // Temporary renaming to avoid conflicts
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

            while (true)
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

                var files = Directory.GetFiles(folder);
                Console.WriteLine($"\n📂 Found {files.Length} file(s):");
                foreach (var f in files)
                    Console.WriteLine("   - " + Path.GetFileName(f));

                Console.Write("\n🔤 OLD filename pattern (use * as wildcard):\n>> ");
                string oldPattern = Console.ReadLine();

                Console.Write("🆕 NEW filename pattern (use * as wildcard or number start):\n>> ");
                string newPattern = Console.ReadLine();

                RenameFilesInFolder(folder, oldPattern, newPattern);

                Console.WriteLine("\nOptions: (R)ename again | (C)hange folder | (E)xit");
                Console.Write(">> ");
                string option = Console.ReadLine().Trim().ToUpper();

                if (option == "E") break;
                if (option == "C") continue;
            }

            Console.WriteLine("\n👋 Goodbye!");
        }
    }
}
