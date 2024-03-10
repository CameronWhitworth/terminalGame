using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
public class OpenCommand : ICommand
{
    public int MaxArguments => 2; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        if (args.Length > 1)
        {
            string fileName = args[1];
            // Ensure .txt extension for reading
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            var fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);
            if (fileMetadata == null)
            {
                terminalManager.AddLinesWithDelay(new List<string> { "File not found: " + fileName });
                return new List<string>(); // Immediate return since AddLinesWithDelay handles the message
            }

            // Check if the file is password protected and if the terminal is not already awaiting password
            if (fileMetadata.IsPasswordProtected)
            {
                // Assuming fileMetadata has a property like .Name or similar that contains the filename
                string fileMetaName = fileMetadata.Name;
                terminalManager.RequestPasswordInput(fileMetaName, isSuccess =>
                {
                    if (isSuccess)
                    {
                        // Password correct, read and display the file content
                        string[] lines = fileMetadata.Content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string>(lines)));

                    }
                    else
                    {
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { "Incorrect password. Access denied. Run command again." }));
                    }
                });
            }
            else
            {
                string[] lines = fileMetadata.Content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
                terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string>(lines)));
            }
        }
        else
        {
            terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { "ERROR: File name not specified" }));
        }
        return new List<string>(); // The actual output is handled asynchronously


    }
}
