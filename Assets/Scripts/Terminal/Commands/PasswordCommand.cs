using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
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

                // Prompt for existing password before removal
                terminalManager.EnablePasswordInputMode(fileName, isSuccess =>
                {
                    if (isSuccess)
                    {
                        fileMetadata.RemovePassword();
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { $"Password removed from file {fileName}." }));
                    }
                    else
                    {
                        terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { "Incorrect password. Operation cancelled." }));
                    }
                });
            }
            else if (args.Length >= 3) // Set a new password
            {
                if (fileMetadata.IsPasswordProtected)
                {
                    // Prompt for existing password before setting a new one
                    terminalManager.EnablePasswordInputMode(fileName, isSuccess =>
                    {
                        if (isSuccess)
                        {
                            string newPassword = args[2];
                            fileMetadata.SetPassword(newPassword);
                            terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { $"Password updated for file {fileName}." }));
                        }
                        else
                        {
                            terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> { "Incorrect password. Operation cancelled." }));
                        }
                    });
                }
                else
                {
                    string newPassword = args[2];
                    fileMetadata.SetPassword(newPassword);
                    return new List<string> { $"Password set for file {fileName}." };
                }
            }
        }
        else
        {
            return new List<string> { "Usage: pass <filename> [newPassword] - newPassword is optional. If not provided, the command will remove the existing password. If setting a new password on a password-protected file, you must enter the old password first." };
        }

        return new List<string>(); // The actual response is handled asynchronously due to the need for password verification
    }
}
