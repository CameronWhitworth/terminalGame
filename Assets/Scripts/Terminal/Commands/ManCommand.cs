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
            // Example entries with detailed descriptions
            {"ls", "NAME:\n     ls\nDESCRIPTION:\n      Lists all directories and files in the current directory, helping users navigate the filesystem."},
            {"cd", "NAME:\n     cd <directory>\nDESCRIPTION:\n      Changes the current directory to <directory>. Essential for navigating through the filesystem."},
            // Add detailed descriptions for other commands here
        };
    }

    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();

        if (args.Length == 0)
        {
            response.AddRange(FormatManualEntry("Usage: man <command>\nDescription: Displays the manual page for <command>. Provide a command name to get its usage and description."));
            return response;
        }

        string command = args[1]; // Assumes args[0] is 'man' itself
        if (commandManuals.TryGetValue(command, out string manual))
        {
            response.AddRange(FormatManualEntry(manual));
        }
        else
        {
            response.AddRange(FormatManualEntry($"No manual entry for '{command}'. Ensure you've typed the command name correctly."));
        }

        return response;
    }

    // New method to format manual entries without ASCII box
    private IEnumerable<string> FormatManualEntry(string text)
    {
        // Split the input text into lines for individual processing
        string[] lines = text.Split('\n');

        foreach (var line in lines)
        {
            // The line is directly added without ASCII box formatting
            yield return line;
        }
    }
}
