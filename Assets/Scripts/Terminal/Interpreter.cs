using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

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
        string[] args = userInput.Split();
        string commandName = args[0];

        // Check for alias
        string aliasCommand;
        if (terminalManager.GetCurrentDirectory().aliases.TryGetValue(commandName, out aliasCommand))
        {
            // If found, parse the alias command as if it was the user input
            args = aliasCommand.Split();
            commandName = args[0]; // Update commandName with the actual command from the alias
        }

        ICommand command = commandRegistry.GetCommand(commandName);
        if (command != null)
        {
            return command.Execute(args, terminalManager);
        }
        else
        {
            response.Add("ERROR Unknown command, Type 'help' for a list of commands");
            return response;
        }
    }

    // public List<string> Interpret(string userInput)
    // {
    //     response.Clear();

    //     string[] args = userInput.Split();

    //     // if(args[0] == "boop")
    //     // {
    //     //     response.Add("Damn that's so cool u typed boop");
    //     //     return response;
    //     // }
    //     if(args[0] == "ascii")
    //     {
    //         LoadTitle("test.txt", "red", 2);
    //         return response;
    //     }
    //     if (args[0] == "clear")
    //     {
    //         terminalManager.ClearScreen();
    //         response.Add("Screen cleared.");
    //         return response;
    //     }
    // }

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

}
