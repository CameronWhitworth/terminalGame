using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class GrepCommand : ICommand
{
    public int MaxArguments => 10; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        // Adjust the condition to account for when previousOutput is provided
        if (args.Length < 2 && previousOutput == null)
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
                    matchWholeWord = true;
                    break;
                default:
                    if (string.IsNullOrEmpty(pattern))
                    {
                        pattern = args[i].Trim('\'', '"');
                    }
                    else
                    {
                        // Only add to filenames if there's no previous output
                        if (previousOutput == null)
                        {
                            fileNames.Add(args[i]);
                        }
                    }
                    break;
            }
        }

        if (string.IsNullOrEmpty(pattern))
        {
            response.Add("Pattern not specified correctly.");
            return response;
        }

        RegexOptions options = RegexOptions.None;
        if (caseInsensitive)
        {
            options |= RegexOptions.IgnoreCase;
        }

        // Modify pattern for whole word match if -w flag is set
        if (matchWholeWord)
        {
            pattern = @"\b" + Regex.Escape(pattern) + @"\b";
        }

        // Use previousOutput if available, else read from files
        if (previousOutput != null)
        {
            // Handle the output from a previous command
            ProcessLines(previousOutput.ToArray(), response, options, pattern, includeFileName: false, includeLineNumbers, includeByteOffset, countMatches, invertMatch, ref totalMatchCount);
        }
        else
        {
            // Original file processing logic here, modified to use a helper method for processing lines
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
                    response.Add($"Error: file {fileName} is password protected. Remove password to perform grep.");
                    continue;
                }

                string[] lines = fileMetadata.Content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                ProcessLines(lines, response, options, pattern, includeFileName, includeLineNumbers, includeByteOffset, countMatches, invertMatch, ref totalMatchCount, fileName);
            }
        }

        if (countMatches)
        {
            response.Add($"{totalMatchCount}");
        }

        return response;
    }

    private void ProcessLines(string[] lines, List<string> response, RegexOptions options, string pattern, bool includeFileName, bool includeLineNumbers, bool includeByteOffset, bool countMatches, bool invertMatch, ref int totalMatchCount, string fileName = null)
    {
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
                    if (includeFileName && fileName != null)
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
}
