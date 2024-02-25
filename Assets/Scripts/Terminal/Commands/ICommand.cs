using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    // Executes the command and returns a list of string responses.
    List<string> Execute(string[] args, TerminalManager terminalManager);
}
