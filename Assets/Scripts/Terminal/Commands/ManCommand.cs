using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class ManCommand : ICommand
{
    public int MaxArguments => 2; 
    private Dictionary<string, string> commandManuals;

    public ManCommand()
    {
        // Initialize command manuals with detailed descriptions
        commandManuals = new Dictionary<string, string>
        {
            {"ls", "NAME:\n     ls\nSYNOPSIS:\n     ls {options}\nDESCRIPTION:\n      Lists all directories and files in the current directory, helping users navigate the filesystem."},
            {"cd", "NAME:\n     cd\nSYNOPSIS:\n     cd {directory}\nDESCRIPTION:\n      Changes the current directory to the specified {directory}, allowing navigation through the filesystem. Use 'cd ..' to move up to the parent directory or 'cd root' to return to the current home directory."},
            {"mkdir", "NAME:\n     mkdir\nSYNOPSIS:\n     mkdir {directory}\nDESCRIPTION:\n      Creates a new directory with the specified name. Essential for organizing files and directories within the filesystem."},
            {"rmdir", "NAME:\n     rmdir\nSYNOPSIS:\n     rmdir {directory}\nDESCRIPTION:\n      Removes the specified directory. The directory needs to be empty for the operation to succeed. Essential for cleaning up the filesystem."},
            {"rm", "NAME:\n     rm\nSYNOPSIS:\n     rm {file}\nDESCRIPTION:\n      Removes the specified file from the filesystem. Can also be used as 'delete' or 'remove'. Use with caution as it permanently deletes files."},
            {"cat / open", "NAME:\n     open\nSYNOPSIS:\n     open {file}\nDESCRIPTION:\n      Opens and displays the content of the specified file. Can also be used as 'cat' to concatenate and display files. cat can also be used to concatenate several files and display their content. Use cat file1 file2 to view multiple files sequentially."},
            {"touch", "NAME:\n     touch\nSYNOPSIS:\n     touch {file}\nDESCRIPTION:\n      Creates a new file if it does not exist or updates the last modified time of a file if it exists. Useful for file management tasks."},
            {"edit", "NAME:\n     edit\nSYNOPSIS:\n     edit {file}\nDESCRIPTION:\n      Opens the specified file for editing. Allows the user to modify the contents of a file within the terminal environment. This command integrates with a simple text editor for improved in-terminal editing experiences."},
            {"mv", "NAME:\n     mv\nSYNOPSIS:\n     mv {source} {destination}\nDESCRIPTION:\n      Moves a file or directory from {source} to {destination}. If {destination} is a directory, the source file or directory is moved into it with its original name. If {destination} specifies a new name (by ending with .txt for files), the source is renamed in the move process."},
            {"cp", "NAME:\n     cp\nSYNOPSIS:\n     cp {source} {destination}\nDESCRIPTION:\n      Copies a file from {source} to {destination}. If {destination} is a directory, the file is copied into it with its original name. If {destination} specifies a new name (must end with .txt for files), the file is copied and renamed to this new name."},
            {"history", "NAME:\n     history\nSYNOPSIS:\n       history {num}\nDESCRIPTION:\n      Displays the list of commands that have been entered in the current session. Useful for reviewing or repeating past commands. A value argument can be passed to print out a specified number of previous command"},
            {"tree", "NAME:\n     tree\nDESCRIPTION:\n      Displays a tree structure of the directory and its subdirectories, providing a visual representation of the filesystem's hierarchy."},
            {"sys", "NAME:\n     sys\nDESCRIPTION:\n      Displays system information including OS version, memory usage, and CPU details. Can also be used as 'sysinfo' or 'system' to access this information."},
            {"calc", "NAME:\n     calc\nSYNOPSIS:\n     calc {expression}\nDESCRIPTION:\n      Evaluates the mathematical expression provided as {expression} and returns the result. Supports basic arithmetic operations like addition (+), subtraction (-), multiplication (*), and division (/). Example usage: 'calc 1 + 2'."},
            {"pass", "NAME:\n     pass\nSYNOPSIS:\n     pass {filename} {newPassword}\nDESCRIPTION:\n      Manages the password protection for a file. If {newPassword} is provided, it sets a new password for the file specified by {filename}. If {newPassword} is not provided, it removes the existing password from the file. To change an existing password, the correct current password must be provided first."},
            {"grep", "NAME:\n     grep\nSYNOPSIS:\n     Usage: grep [options] \"pattern\" {file} {optionalFile2} {optionalFile3} ...\nOPTIONS:\n     -i    Perform case insensitive matching. By default, grep is case sensitive\n     -n    Prefix each line of output with the 1-based line number within its input file\n     -b    Print the 0-based byte offset within the input file before each line of output\nDESCRIPTION:\n      Searches for the specified pattern within each file (or files) and prints the lines where the pattern is found. The pattern needs to be enclosed in quotes if it contains spaces or special characters. This command is powerful for searching through large amounts of text and for filtering relevant lines from files."}
        };
    }

    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        if (args.Length <= 1)
        {
            response.AddRange(FormatManualEntry("Usage: man <command>\nDescription: Displays the manual page for <command>. Provide a command name to get its usage and description.", terminalManager));
            return response;
        }

        string command = args[1];
        if (commandManuals.TryGetValue(command, out string manual))
        {
            response.AddRange(FormatManualEntry(manual, terminalManager));
        }
        else
        {
            response.AddRange(FormatManualEntry($"No manual entry for '{command}'. Ensure you've typed the command name correctly.", terminalManager));
        }

        return response;
    }

    // Modified method to format manual entries with color
    private IEnumerable<string> FormatManualEntry(string text, TerminalManager terminalManager)
    {
        ThemeManager themeManager = terminalManager.GetThemeManager(); // Access the ThemeManager
        string commandColor = themeManager.GetColor("command");
        string descriptionColor = themeManager.GetColor("description");

        // Split the input text into lines for individual processing
        string[] lines = text.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith("NAME:"))
            {
                yield return ColorString(line, commandColor); // Apply command color
            }
            else if (line.StartsWith("DESCRIPTION:"))
            {
                yield return ColorString(line, descriptionColor); // Apply description color
            }
            else if (line.StartsWith("SYNOPSIS:"))
            {
                yield return ColorString(line, descriptionColor); // Apply description color
            }
            else
            {
                yield return line; // No color for other lines
            }
        }
    }

    // Reuse the ColorString method from LsCommand or define it here if not already defined
    private string ColorString(string text, string color)
    {
        return $"<color={color}>{text}</color>"; // Use Unity's rich text format for coloring
    }
}
