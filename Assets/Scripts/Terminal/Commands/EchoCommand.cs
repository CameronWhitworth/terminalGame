using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EchoCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        string echoedText = string.Join(" ", args.Skip(1));
        response.Add(echoedText);
        return response;
    }
}
