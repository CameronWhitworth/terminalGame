using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTheme
{
    public string Name { get; private set; }
    public Dictionary<string, string> Colors { get; private set; }

    // Add a property for default color
    public string DefaultColor { get; set; }

    public ColorTheme(string name)
    {
        Name = name;
        Colors = new Dictionary<string, string>();
        DefaultColor = "#FAA112"; // Set a default color for text, e.g., white
    }

    public string GetColor(string key)
    {
        if (Colors.TryGetValue(key, out string value))
        {
            return value;
        }

        return DefaultColor; // Fallback to default color if the key is not found
    }
}
