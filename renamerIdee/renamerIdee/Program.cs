using System;

namespace renamerIdee
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("     Ultimate File Renamer (version V2.7)");
            Console.WriteLine("==============================================\n");

            string currentFolder = null;

            while (true)
            {
                if (string.IsNullOrEmpty(currentFolder))
                    currentFolder = UserInterface.AskFolder();

                if (currentFolder == null) break;

                UserInterface.ShowFiles(currentFolder);

                Console.WriteLine("\nOptions:");
                Console.WriteLine("  (R) Rename using pattern");
                Console.WriteLine("  (S) Single file rename");
                Console.WriteLine("  (C) Change folder");
                Console.WriteLine("  (E) Exit");
                Console.WriteLine("\n💡 Tip: To remove everything before the first '-', choose 'R' and type REMOVE_PREFIX as OLD pattern.");
                Console.Write(">> ");
                string option = Console.ReadLine().Trim().ToUpper();

                if (option == "E") break;
                if (option == "C") { currentFolder = null; continue; }

                if (option == "S")
                {
                    Console.Write("\nEnter exact file name to rename:\n>> ");
                    string fileToRename = Console.ReadLine();

                    Console.Write("Enter new name for this file:\n>> ");
                    string newName = Console.ReadLine();

                    string fullPath = System.IO.Path.Combine(currentFolder, fileToRename);
                    FileRenamer.RenameSingleFile(fullPath, newName);
                    continue;
                }

                if (option == "R")
                {
                    var oldPattern = UserInterface.AskOldPattern();
                    var newPattern = UserInterface.AskNewPattern();

                    FileRenamer.RenameFilesInFolder(currentFolder, oldPattern, newPattern);
                }
            }

            Console.WriteLine("\n👋 Goodbye!");
        }
    }
}
