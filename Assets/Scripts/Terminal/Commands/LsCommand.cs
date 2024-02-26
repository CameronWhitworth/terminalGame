using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        Directory currentDirectory = terminalManager.GetCurrentDirectory();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager

        foreach (var dir in currentDirectory.subDirectories)
        {
            string directoryColor = themeManager.GetColor("directory");
            response.Add(ColorString("/" + dir.name, directoryColor));
        }

        foreach (var file in currentDirectory.files)
        {
            string fileColor = themeManager.GetColor("file");
            response.Add(ColorString(file.Name, fileColor));
        }

        if (response.Count == 0)
        {
            response.Add("The directory is empty.");
        }

        return response;
    }

    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}
