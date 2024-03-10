using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EchoCommand : ICommand
{
    public int MaxArguments => 100; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        // If there's previous output, echo that instead of the arguments.
        if (previousOutput != null && previousOutput.Count > 0)
        {
            response.Add("foobar");
            response.AddRange(previousOutput);
            
        }
        else
        {
            string echoedText = string.Join(" ", args.Skip(1));
            response.Add(echoedText);
        }
        
        return response;
    }
}
