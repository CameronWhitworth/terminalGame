using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoobCommand : ICommand
{
    public int MaxArguments => 1; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)    {
        List<string> response = new List<string>();
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager


        response.Add("(.)(.)");

        return response;
    }

    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}

