using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
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
}
