using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WcCommand : ICommand
{
    public int MaxArguments => 10;
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        if (args.Length < 2)
        {
            response.Add("Usage: wc <file>");
            return response;
        }

        foreach (var arg in args.Skip(1)) // Skip the command itself
        {
            FileMetadata file = terminalManager.GetCurrentDirectory().GetFileMetadata(arg);
            if (file == null)
            {
                response.Add($"File not found: {arg}");
                continue;
            }

            if (file.IsPasswordProtected)
            {
                response.Add($"Error: File {arg} is password protected. Remove the password to perform wc.");
                continue;
            }

            // Calculate line, word, and character counts
            var lines = file.Content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            int lineCount = lines.Length;
            int wordCount = lines.SelectMany(l => l.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries)).Count();
            int charCount = file.Content.Length; // This includes spaces and newline characters

            response.Add($"{lineCount} {wordCount} {charCount} {arg}");
        }

        return response;
    }
}
