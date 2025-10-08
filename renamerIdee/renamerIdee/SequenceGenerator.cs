using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace renamerIdee
{
	public static class SequenceGenerator
	{
		public static List<(string oldPath, string newPath)> ApplySequence(List<(string oldPath, string newPath)> matchedFiles, string oldName, string newName)
		{
			if (oldName.Contains("*") && !newName.Contains("*") && !newName.Contains("[a]") && !newName.Contains("[A]"))
			{
				int startNum = Helpers.ExtractFirstNumber(newName) ?? 1;
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

			// Handle letter conversion ([a], [A])
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

			return matchedFiles;
		}
	}
}
