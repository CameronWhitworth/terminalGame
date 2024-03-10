using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class AliasCommand : ICommand
{
    public int MaxArguments => 5; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager for styling if needed

        if (args.Length >= 2 && (args[1] == "-l" || args[1] == "--list"))
        {
            var aliases = terminalManager.GetCurrentDirectory().ListAllAliases();
            if (aliases.Count == 0)
            {
                response.Add("No aliases defined.");
                return response;
            }

            foreach (var alias in aliases)
            {
                // Assuming alias format is 'aliasName: command'
                response.Add(alias); // No need for color here unless you want to highlight the entire line
            }
        }
        else if (args.Length >= 2 && (args[1] == "-r" || args[1] == "--remove"))
        {
            if (args.Length == 3)
            {
                string aliasResponse = terminalManager.GetCurrentDirectory().RemoveAlias(args[2]);
                response.Add(aliasResponse);
            }
            else
            {
                response.Add("Usage: alias -r [alias_name]");
            }
        }
        else if (args.Length > 1)
        {
            string userInput = string.Join(" ", args.Skip(1));
            int equalSignIndex = userInput.IndexOf('=');
            if (equalSignIndex > 0)
            {
                string aliasName = userInput.Substring(0, equalSignIndex).Trim();
                string extractedCommand = userInput.Substring(equalSignIndex + 1).Trim();

                if (extractedCommand.StartsWith("'") && extractedCommand.EndsWith("'"))
                {
                    extractedCommand = extractedCommand[1..^1]; // Remove the single quotes
                    string aliasResponse = terminalManager.GetCurrentDirectory().AddAlias(aliasName, extractedCommand);
                    response.Add(aliasResponse);
                }
                else
                {
                    response.Add("ERROR: Alias command must be enclosed in single quotes");
                }
            }
            else
            {
                response.Add("Usage: alias [alias_name] = '[command]'");
            }
        }
        else
        {
            response.Add("Usage: alias -l, alias -r [alias_name], or alias [alias_name] = '[command]'");
        }

        return response;
    }
}
