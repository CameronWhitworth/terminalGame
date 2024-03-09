using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class Directory
{
    public string name;
    public List<Directory> subDirectories;
    public List<FileMetadata> files = new List<FileMetadata>();
    public Directory parent;
    public bool isUserCreated;

    public Dictionary<string, string> aliases = new Dictionary<string, string>();
    public Directory(string name, Directory parent = null, bool isUserCreated = false)
    {
        this.name = name;
        this.subDirectories = new List<Directory>();
        this.files = new List<FileMetadata>();
        this.parent = parent;
        this.isUserCreated = isUserCreated;
    }

    // Add a new subdirectory
    public void AddSubDirectory(string dirName)
    {
        Directory newDir = new Directory(dirName, this);
        subDirectories.Add(newDir);
    }

    public Directory GetSubDirectory(string name)
    {
        // Iterate over subDirectories to find a match by name
        foreach (var subDir in subDirectories)
        {
            if (subDir.name == name)
            {
                return subDir;
            }
        }

        // No matching subdirectory found
        return null;
    }

    public Directory FindSubDirectoryByPath(string path)
    {
        string[] parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        Directory current = this; // Start from the current directory

        foreach (string part in parts)
        {
            // Try to get the next subdirectory in the path
            Directory next = current.GetSubDirectory(part);
            if (next == null)
            {
                // If any part of the path doesn't exist, return null
                return null;
            }
            current = next; // Move to the next part of the path
        }

        // Return the directory found at the end of the path
        return current;
    }

    public string GetFullPath()
    {
        var pathParts = new List<string>();
        var current = this;
        while (current != null)
        {
            pathParts.Add(current.name);
            current = current.parent;
        }
        pathParts.Reverse(); // Reverse to get the correct order from root to current
        return "/" + string.Join("/", pathParts); // Construct the full path string
    }


    // Initialize a basic file system
    public static Directory InitializeFileSystem()
    {
        // Create root directory
        Directory root = new Directory("root");

        // Create 'documents' directory with two subdirectories
        root.AddSubDirectory("documents");
        root.AddTextFileFromRealFile("testFile", "test.txt");
        Directory documents = root.subDirectories.Find(d => d.name == "documents");
        documents.AddSubDirectory("folder1");
        documents.AddTextFileFromRealFile("examplePassword", "beowulf.txt", "test");
        documents.AddTextFileFromRealFile("exampleNoPassword", "beowulf.txt");
        documents.AddSubDirectory("folder2");

        // Create 'id' directory with a text file
        root.AddSubDirectory("id");
        Directory id = root.subDirectories.Find(d => d.name == "id");
        id.AddSubDirectory("folder");

        return root;
    }

    // Method to add a text file with content from a real file within the Unity project's StreamingAssets folder
    public void AddTextFileFromRealFile(string virtualFileName, string realFilePath, string password = null)
    {
        string content = ReadContentFromRealFile(realFilePath);
        if (!string.IsNullOrEmpty(content))
        {
            // If content was successfully read, create the file with that content
            this.CreateFileWithContent(virtualFileName, false, content, password);
        }
    }

    // Read content from a real file located in the Unity project's StreamingAssets folder
    private string ReadContentFromRealFile(string filePath)
    {
        try
        {
            string path = Path.Combine(Application.streamingAssetsPath, filePath);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error reading file: " + e.Message);
        }
        return null;
    }

    // Create a file in the directory with given content
    // Method to create a file in the directory with given content
    public void CreateFileWithContent(string fileName, bool isUserCreated, string content, string password = null)
    {
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }
        // Check if the file already exists based on its name
        if (!files.Any(f => f.Name == fileName))
        {
            var newFile = new FileMetadata(fileName, isUserCreated, content)
            {
                //IsPasswordProtected = !string.IsNullOrEmpty(password),
                Password = password ?? ""
            };
            files.Add(newFile);
        }
    }

    //piping method
    public string WriteToFile(string fileName, string content)
    {
        // Ensure the fileName has the correct extension for consistency
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }

        // Check if the file already exists
        if (FileExists(fileName))
        {
            return "File already exists: " + fileName + ". Use a different name or delete the existing file first.";
        }

        // Create a new file with the given content
        CreateFileWithContent(fileName, true, content);
        return "Output written to: " + fileName;
    }

    // Method to check if a file exists in the current directory
    public bool FileExists(string fileName)
    {
        // Ensure the fileName has the correct extension
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }
        // Search for a file with the matching name
        return files.Any(file => file.Name == fileName);
    }

    // Method to create a new subdirectory
    public string CreateSubDirectory(string dirName)
    {
        if (subDirectories.Exists(d => d.name == dirName))
        {
            return "Directory already exists: " + dirName;
        }
        else
        {
            Directory newDir = new Directory(dirName, this, true); // Set isUserCreated to true
            subDirectories.Add(newDir);
            return "Directory created: " + dirName;
        }
    }

    public string DeleteSubDirectory(string dirName)
    {
        Directory dirToDelete = subDirectories.Find(d => d.name == dirName);

        if (dirToDelete == null)
        {
            return "Directory not found: " + dirName;
        }
        else if (!dirToDelete.isUserCreated)
        {
            return "Cannot delete system directory: " + dirName;
        }
        else
        {
            subDirectories.Remove(dirToDelete);
            return "Directory deleted: " + dirName;
        }
    }

    public string CreateFile(string fileName)
    {
        // Append .txt if not present, this way if a user adds .txt themself it will be ignored
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }

        // Check if the file already exists based on its name
        if (files.Any(f => f.Name == fileName))
        {
            return "File already exists: " + fileName;
        }
        else
        {
            // Create a new FileMetadata object for the file, marking it as user-created
            FileMetadata newFile = new FileMetadata(fileName, true, "testing"); // Initialize with some default content
            files.Add(newFile);

            return "File created: " + fileName;
        }
    }

    // Method to read a file
    public string ReadFile(string fileName)
    {
        // Search for the file by name in the list of FileMetadata objects
        var file = files.FirstOrDefault(f => f.Name == fileName);
        if (file != null)
        {
            // If the file is found, return its content
            return file.Content;
        }
        else
        {
            return "File not found: " + fileName;
        }
    }


    // Method to delete a file with protection check
    public string DeleteFile(string fileName)
    {
        var fileToDelete = files.FirstOrDefault(f => f.Name == fileName);
        if (fileToDelete == null)
        {
            return "File not found: " + fileName;
        }
        else if (!fileToDelete.IsUserCreated)
        {
            return "Cannot delete system file: " + fileName;
        }
        else
        {
            files.Remove(fileToDelete);
            return "File deleted: " + fileName;
        }
    }

    //This is a higher level of delete that over writes system created files ONLY USE WHEN NEED!
    public string PowerDeleteFile(string fileName)
    {
        var fileToDelete = files.FirstOrDefault(f => f.Name == fileName);
        if (fileToDelete == null)
        {
            return "File not found: " + fileName;
        }
        else
        {
            files.Remove(fileToDelete);
            return "File deleted: " + fileName;
        }
    }

    public string AddAlias(string aliasName, string command)
    {
        if (string.IsNullOrWhiteSpace(aliasName) || string.IsNullOrWhiteSpace(command))
        {
            return "Invalid alias or command";
        }

        // Check if the alias conflicts with existing commands
        if (IsCommand(aliasName))
        {
            return $"Cannot create alias '{aliasName}': conflicts with an existing command";
        }

        // Check if the alias already exists
        if (aliases.ContainsKey(aliasName))
        {
            return $"Alias '{aliasName}' already exists";
        }

        // Check for recursive aliases
        if (IsRecursiveAlias(aliasName, command))
        {
            return $"Cannot create alias '{aliasName}': recursive alias detected";
        }

        aliases[aliasName] = command;
        return $"Alias '{aliasName}' created for command '{command}'";
    }

    private bool IsCommand(string aliasName)
    {
        // Replace this with a dict of command stored in a file (This is dog shit but I cba for now)
        var existingCommands = new HashSet<string> { "help", "ls", "cd", "mkdir", "rmdir", "touch", "cat", "system", "alias", "clear", "ascii", "boop", "-r", "-l" }; //PLACEHOLDER!!!!!!!!!
        return existingCommands.Contains(aliasName);
    }

    private bool IsRecursiveAlias(string aliasName, string command)
    {
        // Check if the command starts with the alias itself
        string[] commandParts = command.Split();
        if (commandParts.Length > 0)
        {
            if (commandParts[0] == aliasName)
            {
                return true; // Direct recursion
            }

            if (aliases.TryGetValue(commandParts[0], out string existingCommand))
            {
                return IsRecursiveAlias(aliasName, existingCommand); // Check for indirect recursion
            }
        }

        return false;
    }

    public List<string> ListAllAliases()
    {
        List<string> aliasList = new List<string>();

        foreach (var alias in aliases)
        {
            aliasList.Add($"{alias.Key} -> {alias.Value}");
        }

        if (aliasList.Count == 0)
        {
            aliasList.Add("No aliases defined.");
        }

        return aliasList;
    }

    public string RemoveAlias(string aliasName)
    {
        if (string.IsNullOrWhiteSpace(aliasName))
        {
            return "Invalid alias name";
        }

        if (!aliases.ContainsKey(aliasName))
        {
            return $"Alias '{aliasName}' does not exist";
        }

        aliases.Remove(aliasName);
        return $"Alias '{aliasName}' removed successfully";
    }

    public void SaveFileContent(string fileName, string content)
    {
        // Find the file metadata object for the given fileName
        var fileMetadata = files.FirstOrDefault(f => f.Name == fileName);
        
        // If the file is found, update its content
        if (fileMetadata != null)
        {
            fileMetadata.Content = content;
        }
        else
        {
            // Optionally, handle the case where the file does not exist
            Debug.LogError($"File not found: {fileName}");
        }
    }


    public FileMetadata GetFileMetadata(string fileName)
    {
        // Ensure the fileName has the correct extension for consistency
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }

        // Attempt to find the file with the specified name
        var file = files.FirstOrDefault(f => f.Name == fileName);

        // If found, return the FileMetadata object; otherwise, return null
        return file;
    }
}
