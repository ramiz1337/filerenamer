using System;
using System.IO;

namespace renamerIdee
{
	public static class UserInterface
	{
		public static string AskFolder()
		{
			Console.Write("Enter folder path containing files (or 'exit' to quit):\n>> ");
			string folder = Console.ReadLine();

			if (folder.Equals("exit", StringComparison.OrdinalIgnoreCase))
				return null;

			if (!Directory.Exists(folder))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Folder does not exist. Try again.\n");
				Console.ResetColor();
				return AskFolder();
			}
			return folder;
		}

		public static void ShowFiles(string folder)
		{
			var files = Directory.GetFiles(folder);
			Console.WriteLine($"\n📂 Found {files.Length} file(s):");
			foreach (var f in files)
				Console.WriteLine("   - " + Path.GetFileName(f));
		}

		public static string AskOldPattern()
		{
			Console.Write("\n🔤 OLD filename pattern (use * as wildcard):\n>> ");
			return Console.ReadLine();
		}

		public static string AskNewPattern()
		{
			Console.Write("🆕 NEW filename pattern (use * as wildcard, or [a]/[A] for ASCII letters):\n>> ");
			return Console.ReadLine();
		}

		public static string AskOption()
		{
			Console.WriteLine("\nOptions: (R)ename again | (C)hange folder | (E)xit");
			Console.Write(">> ");
			return Console.ReadLine().Trim().ToUpper();
		}

		public static void PreviewChanges(System.Collections.Generic.List<(string oldPath, string newPath)> matchedFiles)
		{
			Console.WriteLine("\nPreview of changes:");
			foreach (var (oldPath, newPath) in matchedFiles)
				Console.WriteLine($"  {Path.GetFileName(oldPath)}  →  {Path.GetFileName(newPath)}");
		}
	}
}
