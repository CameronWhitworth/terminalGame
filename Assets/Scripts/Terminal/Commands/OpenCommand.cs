using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class OpenCommand : ICommand
{
    
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();
        if (args.Length > 1)
        {
            string fileName = args[1];
            // Ensure .txt extension for reading
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            string fileContent = terminalManager.GetCurrentDirectory().ReadFile(fileName);
            if (fileContent == "File not found: " + fileName)
            {
                response.Add(fileContent);
            }
            else
            {
                string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    // Add each line to the response list
                    response.Add(line);
                }
            }
        }
        else
        {
            response.Add("ERROR: File name not specified");
        }
        return response;
    }
}
