using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CdCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
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
}
