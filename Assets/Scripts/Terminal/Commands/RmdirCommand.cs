using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RmdirCommand : ICommand
{
    public int MaxArguments => 2; 
    // Start is called before the first frame update
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
            dirResponse = terminalManager.GetCurrentDirectory().DeleteSubDirectory(args[1]);
            response.Add(dirResponse);
        }
        else
        {
            response.Add("ERROR: Directory name not specified");
        }
        return response;
    }
}
