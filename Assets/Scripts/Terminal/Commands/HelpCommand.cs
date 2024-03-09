using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)    {
        List<string> response = new List<string>();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager

        // Assuming you have a way to access all commands and their descriptions
        var commands = new Dictionary<string, string>
        {
            {"ls", "List directory contents"},
            {"cd <directory>", "Change the current directory to <directory>"},
            {"mkdir <directory>", "Create a new directory named <directory>"},
            {"rm <file>", "Remove a file or directory"},
            {"touch <file>", "Create a new file named <file>"},
            {"cat <file>", "Display the contents of <file>"},
            {"echo <text>", "Display <text>"},
            {"man <command>", "Display in-depth information of <command>"},
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

