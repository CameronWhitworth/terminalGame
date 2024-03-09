using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MkdirCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
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
}
