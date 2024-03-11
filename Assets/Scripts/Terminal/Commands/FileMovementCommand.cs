using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileMovementCommand : ICommand
{
    public int MaxArguments => 3; 
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        if (args.Length < 3)
        {
            response.Add("ERROR: Missing arguments. Usage: cp <source> <destination>");
            return response;
        }

        string operation = args[0]; // Check if copy or move
        string sourcePath = args[1];
        string destinationPath = args[2];


        // Ensure source file exists
        FileMetadata sourceFileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(sourcePath);
        if (sourceFileMetadata == null)
        {
            response.Add("File not found: " + sourcePath);
            return response;
        }

        // Modify the logic for determining the destination directory or file
        Directory currentDirectory = terminalManager.GetCurrentDirectory();
        Directory destinationDir = null;
        string destinationFileName = "";

        // Check if destination is a directory or a file based on the presence of ".txt"
        if (!destinationPath.EndsWith(".txt"))
        {
            // Assume it's a directory; append "/" if not already present
            if (!destinationPath.EndsWith("/"))
            {
                destinationPath += "/";
            }

            // Attempt to find the directory
            destinationDir = currentDirectory.FindSubDirectoryByPath(destinationPath.TrimEnd('/'));
            if (destinationDir == null)
            {
                response.Add("Destination directory not found: " + destinationPath);
                return response;
            }

            destinationFileName = sourceFileMetadata.Name; // Use the original file's name
        }
        else
        {
            // It's a file; parse directory and file name
            if (destinationPath.Contains("/"))
            {
                string[] pathParts = destinationPath.Split('/');
                string dirPath = string.Join("/", pathParts, 0, pathParts.Length - 1);
                destinationDir = currentDirectory.FindSubDirectoryByPath(dirPath);
                destinationFileName = pathParts[^1].Trim(); // Use the specified file name

                if (destinationDir == null)
                {
                    response.Add("Destination directory not found: " + dirPath);
                    return response;
                }
            }
            else
            {
                destinationFileName = destinationPath.Trim();
                destinationDir = currentDirectory; // No directory path specified, use current directory
            }
        }

        // File copy/move logic (unchanged)
        if (destinationDir != null)
        {
            // Your existing logic for copying or moving the file
            string uniqueDestinationFileName = GetUniqueFileName(destinationDir, destinationFileName);
            destinationDir.CreateFileWithContent(uniqueDestinationFileName, sourceFileMetadata.IsUserCreated, sourceFileMetadata.Content, sourceFileMetadata.Password);

            string destinationDirPath = destinationDir.GetFullPath(); // Get the full path using the new method

            bool fileNameChanged = sourceFileMetadata.Name != destinationFileName;
            string operationVerb = operation == "mv" ? "moved" : "copied";
            string newNameMessage = fileNameChanged ? $" with new name '{destinationFileName}'" : "";

            if (operation == "mv")
            {
                string moveMessage = $"{operationVerb} '{sourcePath}' to '{destinationDirPath}/{destinationFileName}'{newNameMessage}";
                response.Add(moveMessage);
                
                // Existing deletion logic for the 'mv' operation
            }
            else // "cp" operation
            {
                string copyMessage = $"{operationVerb} '{sourcePath}' to '{destinationDirPath}/{destinationFileName}'{newNameMessage}";
                response.Add(copyMessage);
            }

        }
        else
        {
            response.Add("Unexpected error: Destination directory processing failed.");
        }

        if (operation == "mv")
        {
            // If the operation is 'mv', also remove the file from the original location
            var deleteResponse = terminalManager.GetCurrentDirectory().PowerDeleteFile(sourcePath);
            if (!deleteResponse.StartsWith("File deleted"))
            {
                response.Add("Warning: Failed to delete the original file after moving: " + deleteResponse);
            }
        }
        
        return response;
    }

    private string GetUniqueFileName(Directory destinationDir, string originalName)
    {
        string baseName = originalName;
        string extension = ".txt"; // Assuming all files have .txt extension for simplicity

        // Remove the extension for processing
        if (baseName.EndsWith(extension))
        {
            baseName = baseName.Substring(0, baseName.Length - extension.Length);
        }

        string newName = originalName;
        int counter = 1;

        // Check if the file exists, and if so, generate a new name
        while (destinationDir.FileExists(newName))
        {
            newName = $"{baseName}({counter}){extension}";
            counter++;
        }

        return newName;
    }

}
