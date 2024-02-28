using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ManCommand : ICommand
{
    private Dictionary<string, string> commandManuals;

    public ManCommand()
    {
        // Initialize command manuals with detailed descriptions
        commandManuals = new Dictionary<string, string>
        {
            {"ls", "NAME:\n     ls\nSYNOPSIS:\n     ls [options]\nDESCRIPTION:\n      Lists all directories and files in the current directory, helping users navigate the filesystem."},
            {"cd", "NAME:\n     cd\nSYNOPSIS:\n     cd <directory>\nDESCRIPTION:\n      Changes the current directory to the specified <directory>, allowing navigation through the filesystem. Use 'cd ..' to move up to the parent directory or 'cd root' to return to the current home directory."},
            {"mkdir", "NAME:\n     mkdir\nSYNOPSIS:\n     mkdir <directory>\nDESCRIPTION:\n      Creates a new directory with the specified name. Essential for organizing files and directories within the filesystem."},
            {"rmdir", "NAME:\n     rmdir\nSYNOPSIS:\n     rmdir <directory>\nDESCRIPTION:\n      Removes the specified directory. The directory needs to be empty for the operation to succeed. Essential for cleaning up the filesystem."},
            {"rm", "NAME:\n     rm\nSYNOPSIS:\n     rm <file>\nDESCRIPTION:\n      Removes the specified file from the filesystem. Can also be used as 'delete' or 'remove'. Use with caution as it permanently deletes files."},
            {"cat / open", "NAME:\n     open\nSYNOPSIS:\n     open <file>\nDESCRIPTION:\n      Opens and displays the content of the specified file. Can also be used as 'cat' to concatenate and display files. cat can also be used to concatenate several files and display their content. Use cat file1 file2 to view multiple files sequentially."},
            {"touch", "NAME:\n     touch\nSYNOPSIS:\n     touch <file>\nDESCRIPTION:\n      Creates a new file if it does not exist or updates the last modified time of a file if it exists. Useful for file management tasks."},
            {"edit", "NAME:\n     edit\nSYNOPSIS:\n     edit <file>\nDESCRIPTION:\n      Opens the specified file for editing. Allows the user to modify the contents of a file within the terminal environment. This command integrates with a simple text editor for improved in-terminal editing experiences."},
            {"history", "NAME:\n     history\nSYNOPSIS:\n       history <num>\nDESCRIPTION:\n      Displays the list of commands that have been entered in the current session. Useful for reviewing or repeating past commands. A value argument can be passed to print out a specified number of previous command"},
            {"tree", "NAME:\n     tree\nDESCRIPTION:\n      Displays a tree structure of the directory and its subdirectories, providing a visual representation of the filesystem's hierarchy."},
            {"sys", "NAME:\n     sys\nDESCRIPTION:\n      Displays system information including OS version, memory usage, and CPU details. Can also be used as 'sysinfo' or 'system' to access this information."}
        };
    }

    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();

        if (args.Length <= 1) 
        {
            response.AddRange(FormatManualEntry("Usage: man <command>\nDescription: Displays the manual page for <command>. Provide a command name to get its usage and description.", terminalManager));
            return response;
        }

        string command = args[1];
        if (commandManuals.TryGetValue(command, out string manual))
        {
            response.AddRange(FormatManualEntry(manual, terminalManager));
        }
        else
        {
            response.AddRange(FormatManualEntry($"No manual entry for '{command}'. Ensure you've typed the command name correctly.", terminalManager));
        }

        return response;
    }

    // Modified method to format manual entries with color
    private IEnumerable<string> FormatManualEntry(string text, TerminalManager terminalManager)
    {
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager
        string commandColor = themeManager.GetColor("command");
        string descriptionColor = themeManager.GetColor("description");

        // Split the input text into lines for individual processing
        string[] lines = text.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith("NAME:"))
            {
                yield return ColorString(line, commandColor); // Apply command color
            }
            else if (line.StartsWith("DESCRIPTION:"))
            {
                yield return ColorString(line, descriptionColor); // Apply description color
            }
            else if (line.StartsWith("SYNOPSIS:"))
            {
                yield return ColorString(line, descriptionColor); // Apply description color
            }
            else
            {
                yield return line; // No color for other lines
            }
        }
    }

    // Reuse the ColorString method from LsCommand or define it here if not already defined
    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}
