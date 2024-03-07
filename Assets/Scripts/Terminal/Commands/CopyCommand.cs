using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager)
    {
        List<string> response = new List<string>();

        if (args.Length < 3)
        {
            response.Add("ERROR: Missing arguments. Usage: cp <source> <destination>");
            return response;
        }

        string sourcePath = args[1];
        string destinationPath = args[2];
        
        // Ensure source file exists
        FileMetadata sourceFileMetadata = terminalManager.GetCurrentDirectory().GetFileMetadata(sourcePath);
        if (sourceFileMetadata == null)
        {
            response.Add("File not found: " + sourcePath);
            return response;
        }

        // Check if destination is a directory or a file
        Directory currentDirectory = terminalManager.GetCurrentDirectory();
        Directory destinationDir = null;
        string destinationFileName = sourcePath;

        // If destinationPath includes "/", it might be a path or a new file name in a subdirectory
        if (destinationPath.Contains("/"))
        {
            string[] pathParts = destinationPath.Split('/');
            string dirPath = string.Join("/", pathParts, 0, pathParts.Length - 1);
            destinationDir = currentDirectory.FindSubDirectoryByPath(dirPath);
            destinationFileName = pathParts[pathParts.Length - 1];

            // If destinationDir is null, then the directory was not found
            if (destinationDir == null)
            {
                response.Add("Destination not found: " + dirPath);
                return response;
            }
        }
        else
        {
            // If no "/" in destinationPath, it's a file name in the current directory
            destinationFileName = destinationPath;
            destinationDir = currentDirectory;
        }

        // At this point, destinationDir should not be null
        if (destinationDir != null)
        {
            // Copy the file with password protection if it has one
            destinationDir.CreateFileWithContent(destinationFileName, sourceFileMetadata.IsUserCreated, sourceFileMetadata.Content, sourceFileMetadata.Password);
            response.Add($"File '{sourcePath}' copied to '{destinationPath}' with the same password protection.");
        }
        else
        {
            response.Add("Destination directory not found: " + destinationPath);
        }

        return response;
    }
}
