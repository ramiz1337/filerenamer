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

                var oldPattern = UserInterface.AskOldPattern();
                var newPattern = UserInterface.AskNewPattern();

                FileRenamer.RenameFilesInFolder(currentFolder, oldPattern, newPattern);

                var option = UserInterface.AskOption();

                if (option == "E") break;
                if (option == "C") currentFolder = null; // Change folder
                // if option is R → keep same folder, just loop again
            }

            Console.WriteLine("\n👋 Goodbye!");
        }
    }
}
