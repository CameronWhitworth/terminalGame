using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ThemeManager
{
    private List<ColorTheme> themes = new List<ColorTheme>();
    private ColorTheme currentTheme;

    public ThemeManager()
    {
        InitializeThemes();
    }

    private void InitializeThemes()
    {
        // Dark Theme
        var darkTheme = new ColorTheme("Dark");
        darkTheme.Colors.Add("directory", "#4E9A06"); // Green directories
        darkTheme.Colors.Add("file", "#729FCF"); // Blue files
        darkTheme.Colors.Add("command", "#C4A000"); // Yellow-ish color for command
        darkTheme.Colors.Add("description", "#75507B"); // Purple-ish color for description
        themes.Add(darkTheme);

        // Light Theme
        var lightTheme = new ColorTheme("Light");
        lightTheme.Colors.Add("directory", "#005577"); // Darker blue directories
        lightTheme.Colors.Add("file", "#2E3436"); // Almost black files
        lightTheme.Colors.Add("command", "#4E9A06"); // Green for commands
        lightTheme.Colors.Add("description", "#3465A4"); // Soft blue for descriptions
        themes.Add(lightTheme);


        // Retro Theme
        var retroTheme = new ColorTheme("Retro");
        retroTheme.Colors.Add("directory", "#FFB347"); // Orange directories
        retroTheme.Colors.Add("file", "#FFD700"); // Gold files
        retroTheme.Colors.Add("command", "#D3D7CF"); // Light grey for commands
        retroTheme.Colors.Add("description", "#FCE94F"); // Bright yellow for descriptions
        themes.Add(retroTheme);
        
        // Set a default theme
        currentTheme = darkTheme;
    }

    public bool SetTheme(string name)
    {
        var theme = themes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (theme != null)
        {
            currentTheme = theme;
            return true; // Theme found and set
        }
        return false; // Theme not found
    }

    public IEnumerable<string> GetAllThemeNames()
    {
        return themes.Select(t => t.Name);
    }

    public string GetColor(string key)
    {
        return currentTheme.GetColor(key);
    }
}
