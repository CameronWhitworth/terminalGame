using System.Collections.Generic;
using System.Linq;

public class SortCommand : ICommand
{
    public int MaxArguments => 20; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();
        bool reverse = args.Contains("-r");

        if (previousOutput != null && previousOutput.Any())
        {
            response = SortLines(previousOutput, reverse);
        }
        else if (args.Length > 1)
        {
            string fileName = args[1];
            // Before attempting to read the file, check if it's password protected
            FileMetadata fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);
            if (fileMetadata == null)
            {
                response.Add($"File not found: {fileName}");
                return response;
            }

            if (fileMetadata.IsPasswordProtected)
            {
                response.Add($"Error: file {fileName} is password protected. Remove password to perform sort.");
                return response;
            }

            // Now safe to read and sort the file content
            var fileContent = fileMetadata.Content; // Assuming fileMetadata.Content holds the file content as a string
            if (!string.IsNullOrEmpty(fileContent))
            {
                var lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None).ToList();
                response = SortLines(lines, reverse);
            }
            else
            {
                response.Add($"Error reading file: {fileName}");
            }
        }
        else
        {
            response.Add("Usage: sort [-r] [file]... Sorts the input provided. Use -r for reverse order.");
        }

        return response;
    }

    private List<string> SortLines(List<string> lines, bool reverse)
    {
        var filteredLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        if (reverse)
        {
            return filteredLines.OrderByDescending(line => line).ToList();
        }
        else
        {
            return filteredLines.OrderBy(line => line).ToList();
        }
    }
}
