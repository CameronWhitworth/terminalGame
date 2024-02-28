using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; 

public class SwitchThemeCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager

        // Check if the user wants to list all themes
        if (args.Length > 1 && args[1].Equals("-l", StringComparison.OrdinalIgnoreCase))
        {
            var themeNames = themeManager.GetAllThemeNames();
            response.Add("Available themes:");
            foreach (var name in themeNames)
            {
                response.Add(name);
            }
        }
        else if (args.Length > 1)
        {
            string themeName = args[1];
            if (themeManager.SetTheme(themeName))
            {
                response.Add($"Theme switched to {themeName}.");
            }
            else
            {
                response.Add($"Error: Theme '{themeName}' does not exist.");
            }
        }
        else
        {
            response.Add("Please specify a theme name or use '-l' to list all themes.");
        }

        return response;
    }
}
