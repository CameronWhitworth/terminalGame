using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Interpreter : MonoBehaviour
{
    //Colour dict for setting colour of responses if wanted
    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red",     "#ff5897"},
        {"yellow",  "#f2f1b9"},
        {"blue",    "#9ed9d8"}
    };

    TerminalManager terminalManager;
    private void Start()
    {
        terminalManager = FindObjectOfType<TerminalManager>();
    }


    List<string> response = new List<string>();
    public List<string> Interpret(string userInput)
    {
        response.Clear();

        string[] args = userInput.Split();

        //This is for customer user aliasCommands
        if (terminalManager.GetCurrentDirectory().aliases.TryGetValue(args[0], out string aliasCommand))
        {
            userInput = aliasCommand + userInput.Substring(args[0].Length);
            args = userInput.Split();
        }

        if (args[0] == "alias")
        {
            if (args.Length >= 2 && (args[1] == "-l" || args[1] == "--list"))
            {
                return terminalManager.GetCurrentDirectory().ListAllAliases();
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

                return response;
            }
            else
            {
                // Find the position of the first '=' character
                int equalSignIndex = userInput.IndexOf('=');
                if (equalSignIndex > 0)
                {
                    // Extract alias name (trim to remove any leading/trailing whitespaces)
                    string aliasName = userInput.Substring(5, equalSignIndex - 5).Trim();
                    // Extract command, ensuring it's a different variable name if there was a conflict
                    string extractedCommand = userInput.Substring(equalSignIndex + 1).Trim();

                    // Validate the command is enclosed in single quotes
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

                return response;
            }
        }

        if(args[0] == "help")
        {
            ColorListEntry("help", "returns a list of commands");
            ColorListEntry("ascii", "makes pretty art art");
            ColorListEntry("clear", "clears screen");
            ColorListEntry("", "");
            ColorListEntry("ls", "list contents of directory");
            ColorListEntry("cd <directory/folder>", "cd followed by the directory name to enter it");
            ColorListEntry("cd ..", "to go back a folder");
            ColorListEntry("cd root", "force back to root from anywhere");
            ColorListEntry("", "");
            ColorListEntry("alias <custom name>='<command>'", "creates a new alias for a command");
            ColorListEntry("alias -l or alias --list", "lists all defined aliases");
            ColorListEntry("alias -r <alias>", "Removes an alias. Syntax: alias -r [alias_name]");
            
            return response;
        }
        if(args[0] == "boop")
        {
            response.Add("Damn that's so cool u typed boop");
            return response;
        }
        if(args[0] == "ascii")
        {
            LoadTitle("test.txt", "red", 2);
            return response;
        }
        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            response.Add("Screen cleared.");
            return response;
        }
        if (args[0] == "ls")
        {
            if (args.Length == 1)
        {
            // Only execute if 'ls' is not followed by another word
            return ListDirectoryContents();
        }
        else
        {
            // Handle the case where 'ls' is followed by more words
            response.Add("ERROR: 'ls' command does not take any arguments");
            return response;
        }
        }
        if(args[0] == "cd")
        {
            string dirResponse = "";
            if (args.Length > 1)
            {
                if (args[1].EndsWith("/"))
                {
                    args[1] = args[1].Substring(0, args[1].Length - 1);
                }
                terminalManager.ChangeDirectory(args[1], out dirResponse);
                response.Add(dirResponse);
            }
            else
            {
                response.Add("ERROR: No directory specified");
            }
            return response;
        }
        if (args[0] == "mkdir")
        {
            string dirResponse = "";
            if (args.Length > 1)
            {
                if (args[1].EndsWith("/"))
                {
                    args[1] = args[1].Substring(0, args[1].Length - 1);
                }
                dirResponse = terminalManager.GetCurrentDirectory().CreateSubDirectory(args[1]);
                response.Add(dirResponse);
            }
            else
            {
                response.Add("ERROR: Directory name not specified");
            }
            return response;
        }
        if (args[0] == "rmdir")
        {
            string dirResponse = "";
            if (args.Length > 1)
            {
                if (args[1].EndsWith("/"))
                {
                    args[1] = args[1].Substring(0, args[1].Length - 1);
                }
                dirResponse = terminalManager.GetCurrentDirectory().DeleteSubDirectory(args[1]);
                response.Add(dirResponse);
            }
            else
            {
                response.Add("ERROR: Directory name not specified");
            }
            return response;
        }

        if (args[0] == "rm")
        {
            string fileResponse = "";
            if (args.Length > 1)
            {
                string fileName = args[1];
                // Ensure .txt extension for deletion
                if (!fileName.EndsWith(".txt"))
                {
                    fileName += ".txt";
                }

                fileResponse = terminalManager.GetCurrentDirectory().DeleteFile(fileName);
                response.Add(fileResponse);
            }
            else
            {
                response.Add("ERROR: File name not specified");
            }
            return response;
        }

        //Create text file
        if (args[0] == "touch")
        {
            string fileResponse = "";
            if (args.Length > 1)
            {
                fileResponse = terminalManager.GetCurrentDirectory().CreateFile(args[1]);
                response.Add(fileResponse);
            }
            else
            {
                response.Add("ERROR: File name not specified");
            }
            return response;
        }
        //open text file
        if (args[0] == "cat" || args[0] == "open")
        {
            string fileResponse = "";
            if (args.Length > 1)
            {
                string fileName = args[1];
                // Ensure .txt extension for reading
                if (!fileName.EndsWith(".txt"))
                {
                    fileName += ".txt";
                }

                fileResponse = terminalManager.GetCurrentDirectory().ReadFile(fileName);
                response.Add(fileResponse);
            }
            else
            {
                response.Add("ERROR: File name not specified");
            }
            return response;
        }
        if (args[0] == "system")
        {
            if (args.Length >= 3 && args[1] == "--set" && args[2] == "fontsize")
            {
                if (args.Length == 4 && int.TryParse(args[3], out int textSize))
                {
                    terminalManager.ChangeTextSize(textSize);
                    response.Add($"Text size set to {textSize}");
                }
                else
                {
                    response.Add("ERROR: Invalid text size");
                }
            }
            else
            {
                response.Add("ERROR: Invalid option command");
            }
            return response;
        }
         if (args[0] == "edit")
        {
            string dirResponse = "";
            if (args.Length > 1)
            {
                terminalManager.EditFile(args[1], out dirResponse);
                response.Add(dirResponse);
                return new List<string>();
            }
            else
            {
                response.Add("ERROR: No file specified");
            }
            return response;
        }
        else
        {
            response.Add("ERROR Unknown command, Type 'help' for a list of commands");
            return response;
        }

    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    void ColorListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["red"]) + ": " + ColorString(b, colors["yellow"]));
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

    // Method to list contents of the current directory
    private List<string> ListDirectoryContents()
    {
        List<string> response = new List<string>();

        Directory currentDirectory = terminalManager.GetCurrentDirectory();

        // List all subdirectories
        foreach (var dir in currentDirectory.subDirectories)
        {
            response.Add(ColorString("/" + dir.name, colors["blue"]));
        }

        // List all files
        foreach (var file in currentDirectory.files)
        {
            response.Add(file);
        }

        if(response.Count == 0)
        {
            response.Add(currentDirectory + " is empty.");
        }

        return response;
    }

}
