using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
public class HistoryCommand : ICommand
{
    public int MaxArguments => 2; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
        // Default to showing all commands if no argument is provided
        int commandCount = terminalManager.GetCommandHistory().Count;

        // Check if an argument is provided
        if (args.Length > 1)
        {
            // Attempt to parse the argument as an integer
            if (int.TryParse(args[1], out int requestedCount) && requestedCount > 0)
            {
                commandCount = Math.Min(requestedCount, commandCount);
            }
            else
            {
                // If parsing fails or the number is not positive, return an error message
                response.Add("Error: Arguments passed to 'history' need to be a positive numbered value.");
                return response;
            }
        }

        return ListCommandHistory(commandCount, terminalManager);

    }
    private List<string> ListCommandHistory(int commandCount, TerminalManager terminalManager)
    {
        IReadOnlyList<string> history = terminalManager.GetCommandHistory();
        List<string> response = new List<string>();

        // Calculate the starting index based on the requested command count
        int startIndex = Math.Max(0, history.Count - commandCount);

        for (int i = startIndex; i < history.Count; i++)
        {
            // Adjust the display index to be 1-based and relative to the subset of commands being displayed
            response.Add($"{i - startIndex + 1}: {history[i]}");
        }

        if (response.Count == 0)
        {
            response.Add("No commands have been entered yet.");
        }

        return response;
    }
}
