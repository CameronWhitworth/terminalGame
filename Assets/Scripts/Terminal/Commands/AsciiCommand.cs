using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
public class AsciiCommand : ICommand
{
    List<string> response = new List<string>();
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        
        LoadTitle("test.txt", "red", 2);
        return response;
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
    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }
    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red",     "#ff5897"},
        {"yellow",  "#f2f1b9"},
        {"blue",    "#9ed9d8"}
    };
}
