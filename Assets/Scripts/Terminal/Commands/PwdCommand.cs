using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PwdCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        
        // Get the current directory from the terminal manager
        Directory currentDirectory = terminalManager.GetCurrentDirectory();
        
        // Build the full path of the current directory
        string fullPath = BuildFullPath(currentDirectory);
        
        // Add the full path to the response
        response.Add(fullPath);
        
        return response;
    }
    
    private string BuildFullPath(Directory directory)
    {
        // Recursive function to construct the full path from the root to the current directory
        if (directory.parent != null)
        {
            return BuildFullPath(directory.parent) + "/" + directory.name;
        }
        else
        {
            // Assuming the root directory's name is "root", but adjust if necessary
            return directory.name;
        }
    }
}