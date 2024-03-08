using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class GrepCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();

        if (args.Length < 3)
        {
            response.Add("Usage: grep [options] \"pattern\" <file> <optionalFile2> <optionalFile3> ...");
            return response;
        }

        // Initialize flags
        bool caseInsensitive = false;
        bool includeLineNumbers = false;
        bool includeByteOffset = false;
        string pattern = string.Empty;
        List<string> fileNames = new List<string>();

        // Process arguments to separate options, pattern, and filenames
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-i":
                    caseInsensitive = true;
                    break;
                case "-n":
                    includeLineNumbers = true;
                    break;
                case "-b":
                    includeByteOffset = true;
                    break;
                default:
                    if (string.IsNullOrEmpty(pattern) && (args[i].StartsWith("\"") || args[i].StartsWith("'")))
                    {
                        pattern = args[i].Trim('\'', '"');
                    }
                    else
                    {
                        fileNames.Add(args[i]);
                    }
                    break;
            }
        }

        // Check if pattern was set
        if (string.IsNullOrEmpty(pattern))
        {
            response.Add("Pattern not specified correctly.");
            return response;
        }

        // Regex options
        RegexOptions options = RegexOptions.None;
        if (caseInsensitive)
        {
            options |= RegexOptions.IgnoreCase;
        }

        // Iterate over each file name provided
        foreach (string fileName in fileNames)
        {
            FileMetadata fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);
            if (fileMetadata == null)
            {
                response.Add($"File not found: {fileName}");
                continue; // Skip to the next file
            }

            string[] lines = fileMetadata.Content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int byteOffset = 0;
            bool matchFound = false;
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (Regex.IsMatch(lines[lineIndex], pattern, options))
                {
                    StringBuilder output = new StringBuilder();
                    if (includeByteOffset)
                    {
                        output.Append($"{byteOffset}:");
                    }
                    if (includeLineNumbers)
                    {
                        output.Append($"{lineIndex + 1}:");
                    }
                    output.Append(lines[lineIndex]);
                    response.Add(output.ToString());
                    matchFound = true;
                }
                byteOffset += Encoding.UTF8.GetByteCount(lines[lineIndex] + "\n");
            }

            if (!matchFound)
            {
                response.Add($"No matches found in: {fileName}");
            }
        }

        return response;
    }
}
