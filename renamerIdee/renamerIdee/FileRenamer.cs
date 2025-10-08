using System;
using System.Collections.Generic;
using System.IO;

namespace renamerIdee
{
	public static class FileRenamer
	{
		public static void RenameFilesInFolder(string folderPath, string oldName, string newName)
		{
			var matchedFiles = PatternMatcher.MatchFiles(folderPath, oldName, newName);
			matchedFiles = SequenceGenerator.ApplySequence(matchedFiles, oldName, newName);

			if (matchedFiles.Count == 0)
			{
				Console.WriteLine("⚠️ No files matched.");
				return;
			}

			UserInterface.PreviewChanges(matchedFiles);

			Console.Write("\nApply these changes? (Y/N): ");
			if (Console.ReadLine().Trim().ToUpper() == "Y")
				ApplyChanges(matchedFiles);
			else
				Console.WriteLine("❌ Operation cancelled.");
		}

		private static void ApplyChanges(List<(string oldPath, string newPath)> matchedFiles)
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
	}
}
