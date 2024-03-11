using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class UniqCommand : ICommand
{
    public int MaxArguments => 10; // Adjust as necessary for your command's needs

    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        // Parse options
        bool countOccurrences = args.Contains("-c");
        bool caseInsensitive = args.Contains("-i");
        bool printDuplicatesOnly = args.Contains("-d");
        bool printUniquesOnly = args.Contains("-u");

        List<string> linesToProcess = new List<string>();

        if (previousOutput != null && previousOutput.Any())
        {
            linesToProcess = previousOutput.Select(line => StripRichTextTags(line)).ToList();
        }
        else if (args.Length > 1)
        {
            // When input is not piped, but a file is specified
            string fileName = args[1];
            var fileContent = terminalManager.GetCurrentDirectory().ReadFile(fileName);
            if (fileContent != null)
            {
                linesToProcess = fileContent.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None)
                                            .Select(line => StripRichTextTags(line))
                                            .ToList();
            }
            else
            {
                return new List<string> { $"File not found: {fileName}" };
            }
        }

        // Processing lines
        string lastLine = null;
        int occurrenceCount = 0;
        foreach (var line in linesToProcess)
        {
            string currentLine = caseInsensitive ? line.ToLower() : line;
            string compareLine = caseInsensitive ? (lastLine?.ToLower()) : lastLine;

            if (currentLine != compareLine)
            {
                if (lastLine != null && (!printUniquesOnly || occurrenceCount == 1) && (!printDuplicatesOnly || occurrenceCount > 1))
                {
                    if (countOccurrences)
                    {
                        response.Add($"{occurrenceCount} {lastLine}");
                    }
                    else
                    {
                        response.Add(lastLine);
                    }
                }
                occurrenceCount = 1;
                lastLine = line;
            }
            else
            {
                occurrenceCount++;
            }
        }

        // Ensure the last line is processed correctly
        if (lastLine != null && (!printUniquesOnly || occurrenceCount == 1) && (!printDuplicatesOnly || occurrenceCount > 1))
        {
            if (countOccurrences)
            {
                response.Add($"{occurrenceCount} {lastLine}");
            }
            else
            {
                response.Add(lastLine);
            }
        }

        return response;
    }

    private string StripRichTextTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}
