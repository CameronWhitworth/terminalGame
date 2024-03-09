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
            response.Add("Usage: grep [options] \"pattern\" <file> [<file2> ...]");
            return response;
        }

        // Initialize flags
        bool caseInsensitive = false;
        bool includeLineNumbers = false;
        bool includeByteOffset = false;
        bool countMatches = false;
        bool invertMatch = false;
        bool includeFileName = false;
        bool matchWholeWord = false; // New flag for whole word matching
        string pattern = string.Empty;
        List<string> fileNames = new List<string>();
        int totalMatchCount = 0;

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
                case "-c":
                    countMatches = true;
                    break;
                case "-v":
                    invertMatch = true;
                    break;
                case "-H":
                    includeFileName = true;
                    break;
                case "-w":
                    matchWholeWord = true; // Process whole word match flag
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

        if (string.IsNullOrEmpty(pattern))
        {
            response.Add("Pattern not specified correctly.");
            return response;
        }

        // Modify pattern for whole word match if -w flag is set
        if (matchWholeWord)
        {
            pattern = @"\b" + Regex.Escape(pattern) + @"\b";
        }

        RegexOptions options = RegexOptions.None;
        if (caseInsensitive)
        {
            options |= RegexOptions.IgnoreCase;
        }

        foreach (string fileName in fileNames)
        {
            FileMetadata fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);
            if (fileMetadata == null)
            {
                response.Add($"File not found: {fileName}");
                continue;
            }

            if (fileMetadata.IsPasswordProtected)
            {
                response.Add($"Error: file {fileName} has password protection. Remove password from file to perform Grep command.");
                continue;
            }

            string[] lines = fileMetadata.Content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int byteOffset = 0;
            int matchCount = 0;
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                bool isMatch = Regex.IsMatch(lines[lineIndex], pattern, options);
                if (invertMatch) isMatch = !isMatch;

                if (isMatch)
                {
                    matchCount++;
                    if (!countMatches)
                    {
                        StringBuilder output = new StringBuilder();
                        if (includeFileName)
                        {
                            output.Append($"{fileName}:");
                        }
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
                    }
                }
                byteOffset += Encoding.UTF8.GetByteCount(lines[lineIndex] + "\n");
            }

            if (countMatches)
            {
                totalMatchCount += matchCount;
            }
            else if (matchCount == 0 && !countMatches)
            {
                response.Add($"No matches found in: {fileName}");
            }
        }
        if (countMatches)
        {
            response.Add($"{totalMatchCount}"); // Add total match count to the response
        }

        return response;
    }
}
