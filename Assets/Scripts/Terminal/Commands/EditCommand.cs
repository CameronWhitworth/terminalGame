using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();

        if (args.Length > 1)
        {
            string fileName = args[1];
            var fileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(fileName);
            
            // Check if file exists and is password protected
            if (fileMetadata != null)
            {
                if (fileMetadata.IsPasswordProtected)
                {
                    // Enable password input mode for the file
                    terminalManager.EnablePasswordInputMode(fileName, isSuccess =>
                    {
                        // This callback is invoked after password input is processed
                        if (isSuccess)
                        {
                            // Proceed with file editing
                            terminalManager.EditFile(args[1], out string dirResponse);
                            terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> {dirResponse}));
                        }
                        else
                        {
                            // Handle incorrect password or cancellation
                            var errorMessage = "Incorrect password. Editing cancelled.";
                            terminalManager.StartCoroutine(terminalManager.AddLinesWithDelay(new List<string> {errorMessage}));
                        }
                    });
                }
                else if (!fileMetadata.IsPasswordProtected)
                {
                    // File is not password protected, proceed with editing
                    terminalManager.EditFile(args[1], out string dirResponse);
                    return new List<string> {dirResponse};
                }
            }
            else
            {
                response.Add("ERROR: File not found");
            }
        }
        else
        {
            response.Add("ERROR: No file specified");
        }

        return response; // This might need to be adjusted based on your implementation
    }
}