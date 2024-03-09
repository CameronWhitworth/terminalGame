using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    // Executes the command and optionally accepts input from a previous command in the pipeline.
    // args: The arguments passed to the command.
    // terminalManager: Reference to the TerminalManager handling this command.
    // previousOutput: The output from the previous command in the pipeline, if any.
    List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null);
}
