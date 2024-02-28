using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager
{
    public GameObject responceLinePrefab;
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
        darkTheme.DefaultColor = "#FFFFFF"; 
        darkTheme.Colors.Add("directory", "#4E9A06"); // Green directories
        darkTheme.Colors.Add("file", "#729FCF"); // Blue files
        darkTheme.Colors.Add("command", "#C4A000"); // Yellow-ish color for command
        darkTheme.Colors.Add("description", "#75507B"); // Purple-ish color for description
        themes.Add(darkTheme);

        // Light Theme
        var lightTheme = new ColorTheme("Light");
        lightTheme.DefaultColor = "#000000";
        lightTheme.Colors.Add("directory", "#005577"); // Darker blue directories
        lightTheme.Colors.Add("file", "#2E3436"); // Almost black files
        lightTheme.Colors.Add("command", "#4E9A06"); // Green for commands
        lightTheme.Colors.Add("description", "#3465A4"); // Soft blue for descriptions
        themes.Add(lightTheme);


        // Retro Theme
        var retroTheme = new ColorTheme("Retro");
        retroTheme.DefaultColor = "#CE5C00";
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
            ApplyThemeToTerminal(); // Apply theme colors inside this method
            return true; // Theme found and set
        }
        return false; // Theme not found
    }

    private void ApplyThemeToTerminal()
    {
        // Find all Text components in children of a certain GameObject
        Text[] textElements = GameObject.Find("Terminal").GetComponentsInChildren<Text>();

        foreach (var textElement in textElements)
        {
            if (textElement.name == "ResponceText" || textElement.name == "DirectoryText" || textElement.name == "UserInputText") // Use the specific name to identify your text elements
            {
                // Apply the color
                string hexColor = currentTheme.GetColor("default"); // Use the key for your default color
                Color newColor;
                if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
                {
                    textElement.color = newColor;
                }
            } 
        }
    }

    public void ApplyColorToText(Text textElement, string elementType = "default")
    {
        string hexColor = GetColor(elementType); // Get color based on element type, default if not specified
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            textElement.color = newColor;
        }
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
