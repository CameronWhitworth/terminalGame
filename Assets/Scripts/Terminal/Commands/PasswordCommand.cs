using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        // Check if the filename and password (optional) are provided
        if (args.Length >= 2)
        {
            string fileName = args[1];
            var fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);

            if (fileMetadata == null)
            {
                return new List<string> { $"ERROR: File {fileName} not found." };
            }

            if (args.Length == 2) // Remove password
            {
                if (!fileMetadata.IsPasswordProtected)
                {
                    return new List<string> { $"File {fileName} is not password protected." };
                }

                // Prompt for password before removal
                terminalManager.EnablePasswordInputMode(fileName, isSuccess =>
                {
                    if (isSuccess)
                    {
                        fileMetadata.RemovePassword();
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { $"Password removed from file {fileName}." }));
                    }
                    else
                    {
                        var errorMessage = "Incorrect password. Operation cancelled.";
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> {errorMessage}));
                    }
                });
            }
            else if (args.Length >= 3) // Set a new password
            {
                string newPassword = args[2];
                fileMetadata.SetPassword(newPassword);
                return new List<string> { $"Password set for file {fileName}." };
            }
        }
        else
        {
            return new List<string> { "Usage: pass <filename> [newPassword] - newPassword is optional. If not provided, the command will remove the existing password." };
        }

        return new List<string>(); // The actual response is handled asynchronously
    }
}
