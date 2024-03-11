using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RmCommand : ICommand
{
    public int MaxArguments => 2; 
    // Start is called before the first frame update
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
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
}
