using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCommand : ICommand
{
    public int MaxArguments => 1; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
        terminalManager.ClearScreen();
        response.Add("");
        return response;
    }
}
