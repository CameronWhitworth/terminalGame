using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager

        // Assuming you have a way to access all commands and their descriptions
        var commands = new Dictionary<string, string>
        {
            {"ls", "List directory contents"},
            {"cd", "Change the current directory"},
            {"mkdir", "Create a new directory"},
            {"rm", "Remove a file or directory"},
            {"touch", "Create a new file"},
            {"cat", "Display the contents of a file"},
            {"echo", "Display a line of text"},
            {"help", "Show this help message"},
            {"switchtheme", "Switch the terminal color theme"},
            // Add other commands and their descriptions here
        };

        string commandColor = themeManager.GetColor("command"); // Fetch color for commands
        string descriptionColor = themeManager.GetColor("description"); // Fetch color for descriptions

        foreach (var command in commands)
        {
            string commandText = ColorString(command.Key, commandColor);
            string descriptionText = ColorString(command.Value, descriptionColor);
            response.Add($"{commandText}: {descriptionText}");
        }

        return response;
    }

    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}

