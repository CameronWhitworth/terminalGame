using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        Directory currentDirectory = terminalManager.GetCurrentDirectory();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager

        // Start the tree from the current directory
        response.Add(ColorString(currentDirectory.name, themeManager.GetColor("directory")));
        GenerateTree(currentDirectory, "", response, themeManager, true);

        return response;
    }

    private void GenerateTree(Directory directory, string indent, List<string> response, ThemeManager themeManager, bool isLast)
    {
        // Process all subdirectories
        for (int i = 0; i < directory.subDirectories.Count; i++)
        {
            bool isLastDirectory = (i == directory.subDirectories.Count - 1) && (directory.files.Count == 0);
            Directory subDir = directory.subDirectories[i];
            response.Add(indent + (isLastDirectory ? "└─" : "├─") + ColorString(subDir.name, themeManager.GetColor("directory")));
            GenerateTree(subDir, indent + (isLastDirectory ? "  " : "│ "), response, themeManager, isLastDirectory);
        }

        // Process all files in the directory
        for (int j = 0; j < directory.files.Count; j++)
        {
            bool isLastFile = (j == directory.files.Count - 1);
            FileMetadata file = directory.files[j];
            response.Add(indent + (isLastFile ? "└─" : "├─") + ColorString(file.Name, themeManager.GetColor("file")));
        }
    }

    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}
