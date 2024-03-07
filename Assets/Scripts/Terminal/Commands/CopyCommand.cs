using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        string destinationFileName = "";

        // If destinationPath includes "/", it might be a path or a new file name in a subdirectory
        if (destinationPath.Contains("/"))
        {
            string[] pathParts = destinationPath.Split('/');
            string dirPath = string.Join("/", pathParts, 0, pathParts.Length - 1);
            destinationDir = currentDirectory.FindSubDirectoryByPath(dirPath);
            destinationFileName = pathParts[pathParts.Length - 1].Trim();

            // If the destination file name is empty, use the source file's name
            if (string.IsNullOrEmpty(destinationFileName))
            {
                destinationFileName = sourceFileMetadata.Name; // Use the original file name
            }

            // If destinationDir is null, then the directory was not found
            if (destinationDir == null)
            {
                response.Add("Destination not found: " + dirPath);
                return response;
            }
        }
        else
        {
            destinationFileName = destinationPath.Trim();

            // If destination file name is empty, use the source file's name
            if (string.IsNullOrEmpty(destinationFileName))
            {
                destinationFileName = sourceFileMetadata.Name; // Use the original file name
            }

            destinationDir = currentDirectory;
        }

        // At this point, destinationDir should not be null
       if (destinationDir != null)
        {
            // Generate a unique file name to avoid conflicts
            string uniqueDestinationFileName = GetUniqueFileName(destinationDir, destinationFileName);

            // Copy the file with password protection if it has one, using the unique file name
            destinationDir.CreateFileWithContent(uniqueDestinationFileName, sourceFileMetadata.IsUserCreated, sourceFileMetadata.Content, sourceFileMetadata.Password);
            response.Add($"File '{sourcePath}' copied to '{uniqueDestinationFileName}'");
        }
        else
        {
            response.Add("Destination directory not found: " + destinationPath);
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
