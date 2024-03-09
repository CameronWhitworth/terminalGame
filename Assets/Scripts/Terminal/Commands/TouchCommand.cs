using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
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
}
