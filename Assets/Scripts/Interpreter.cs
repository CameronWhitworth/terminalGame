using System.Collections;
using System.Collections.Generic;
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

        if(args[0] == "help")
        {
            ColorListEntry("help", "returns a list of commands");
            ColorListEntry("ascii", "makes pretty art art");
            ColorListEntry("clear", "clears screen");
            ColorListEntry("ls", "list contents of directory");
            ColorListEntry("cd", "cd followed by the file directory name to enter");
            ColorListEntry("cd ..", "to go back a folder");
            ColorListEntry("//", "force back to root from anywhere");
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
