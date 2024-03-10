using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class Interpreter : MonoBehaviour
{
    TerminalManager terminalManager;
    private CommandRegistry commandRegistry;
    private void Start()
    {
        terminalManager = FindObjectOfType<TerminalManager>();
        commandRegistry = new CommandRegistry();
    }


    List<string> response = new List<string>();
    public List<string> Interpret(string userInput)
    {
        response.Clear();

        string fileName = null;
        bool isRedirectingOutput = false;

        // Check for output redirection in the userInput
        if (userInput.Contains(">"))
        {
            var parts = userInput.Split(new[] {'>'}, 2);
            userInput = parts[0].Trim();
            var fileNameParts = parts[1].Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            
            if (fileNameParts.Length == 1)
            {
                fileName = fileNameParts[0];
                isRedirectingOutput = true;
            }
            else
            {
                return new List<string> { "ERROR: Too many arguments after redirection operator '>'" };
            }
        }

        // Split the userInput into separate commands based on the pipe symbol
        var commandParts = userInput.Split('|').Select(part => part.Trim()).ToArray();
        List<string> previousCommandOutput = null;

        foreach (var part in commandParts)
        {
            string[] args = part.Split();
            string commandName = args[0];

            // Check for alias
            string aliasCommand;
            if (terminalManager.GetCurrentDirectory().aliases.TryGetValue(commandName, out aliasCommand))
            {
                args = aliasCommand.Split();
                commandName = args[0];
            }

            ICommand command = commandRegistry.GetCommand(commandName);
            if (command != null)
            {
                if (args.Length > command.MaxArguments)
                {
                    return new List<string> { "ERROR: Too many arguments for command '" + commandName + "'" };
                }
                previousCommandOutput = command.Execute(args, terminalManager, previousCommandOutput);

                // If there's another command in the pipeline, strip rich text tags from the output
                if (commandParts.Length > 1)
                {
                    previousCommandOutput = previousCommandOutput.Select(line => StripRichTextTags(line)).ToList();
                }
            }
            else
            {
                string suggestion = SuggestSimilarCommand(commandName);
                if (suggestion != null)
                {
                    response.Add($"ERROR: Unknown command '{commandName}'. Did you mean '{suggestion}'?");
                }
                else
                {
                    response.Add($"ERROR: Unknown command '{commandName}'.");
                }
                response.Add("Type 'help' for a list of commands.");
                return response;
            }
        }

        // Handle output redirection if a file name was provided
        if (isRedirectingOutput && !string.IsNullOrEmpty(fileName))
        {
            // If output redirection is used, strip rich text tags from the output
            var plainTextOutput = previousCommandOutput.Select(line => StripRichTextTags(line)).ToList();
            var writeResult = terminalManager.GetCurrentDirectory().WriteToFile(fileName, string.Join("\n", plainTextOutput));
            return new List<string> {writeResult};
        }

        // The output of the last command in the pipeline is the final response
        return previousCommandOutput ?? new List<string> {"ERROR: Command pipeline execution failed."};
    }

    private string StripRichTextTags(string input)
    {
        // Regex to find and remove any rich text tags
        return Regex.Replace(input, "<.*?>", string.Empty);
    }


    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));

        for(int i = 0; i < spacing; i ++)
        {
            response.Add("");
        }

        while(!file.EndOfStream)
        {
            response.Add(ColorString(file.ReadLine(), colors[color]));
        }

        for(int i = 0; i < spacing; i ++)
        {
            response.Add("");
        }

        file.Close();
    }

    //Colour dict for setting colour of responses if wanted
    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red",     "#ff5897"},
        {"yellow",  "#f2f1b9"},
        {"blue",    "#9ed9d8"}
    };

    private string SuggestSimilarCommand(string inputCommand)
    {
        // Only suggest for commands that are at least 2 characters long
        if (inputCommand.Length < 2)
        {
            return null;
        }

        var registeredCommands = commandRegistry.GetAllCommands(); // Ensure you have a method to get all registered command names
        string closestMatch = null;
        int smallestDistance = int.MaxValue;

        foreach (var command in registeredCommands)
        {
            int distance = ComputeLevenshteinDistance(inputCommand, command);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestMatch = command;
            }
        }

        // Threshold for suggesting a command could be adjusted based on your preference
        return smallestDistance <= 2 ? closestMatch : null; 
    }

    private int ComputeLevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.IsNullOrEmpty(t) ? 0 : t.Length;
        }

        if (string.IsNullOrEmpty(t))
        {
            return s.Length;
        }

        int[] v0 = new int[t.Length + 1];
        int[] v1 = new int[t.Length + 1];

        for (int i = 0; i < v0.Length; i++)
            v0[i] = i;

        for (int i = 0; i < s.Length; i++)
        {
            v1[0] = i + 1;

            for (int j = 0; j < t.Length; j++)
            {
                int cost = s[i] == t[j] ? 0 : 1;
                v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
            }

            for (int j = 0; j < v0.Length; j++)
                v0[j] = v1[j];
        }

        return v1[t.Length];
    }

}
