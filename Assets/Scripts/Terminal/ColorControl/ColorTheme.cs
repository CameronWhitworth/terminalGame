using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTheme
{
    public string Name { get; set; }
    public Dictionary<string, string> Colors { get; set; }

    public ColorTheme(string name)
    {
        Name = name;
        Colors = new Dictionary<string, string>();
    }

    public string GetColor(string key)
    {
        Colors.TryGetValue(key, out string color);
        return color ?? "#FFFFFF"; // Default color if not found
    }
}
