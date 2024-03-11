using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Required for Mathf

public class DiffCommand : ICommand
{
    public int MaxArguments => 3; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        if (args.Length < 3)
        {
            response.Add("Usage: diff <file1> <file2>");
            return response;
        }

        string fileName1 = args[1];
        string fileName2 = args[2];

        FileMetadata file1 = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName1);
        FileMetadata file2 = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName2);

        if (file1 == null || file2 == null)
        {
            response.Add("Error: One or both files do not exist.");
            return response;
        }

        // Check if either file is password protected
        if (file1.IsPasswordProtected || file2.IsPasswordProtected)
        {
            response.Add("Error: One or both files are password protected. Remove the password to perform diff.");
            return response;
        }

        // Split the contents into lines for comparison
        var lines1 = file1.Content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None).ToList();
        var lines2 = file2.Content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None).ToList();

        // Example simple diff logic (can be replaced with more sophisticated diff algorithm)
        for (int i = 0; i < Mathf.Max(lines1.Count, lines2.Count); i++)
        {
            if (i >= lines1.Count)
            {
                response.Add($"+ {lines2[i]}");
            }
            else if (i >= lines2.Count)
            {
                response.Add($"- {lines1[i]}");
            }
            else if (lines1[i] != lines2[i])
            {
                response.Add($"- {lines1[i]}");
                response.Add($"+ {lines2[i]}");
            }
        }

        return response;
    }
}
