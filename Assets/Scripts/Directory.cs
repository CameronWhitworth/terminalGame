using System.Collections.Generic;

[System.Serializable]
public class Directory
{
    public string name;
    public List<Directory> subDirectories;
    public List<string> files;
    public Directory parent;
    public bool isUserCreated;
    private Dictionary<string, string> fileContents;

    public Dictionary<string, string> aliases = new Dictionary<string, string>();
    public Directory(string name, Directory parent = null, bool isUserCreated = false)
    {
        this.name = name;
        this.subDirectories = new List<Directory>();
        this.files = new List<string>();
        this.parent = parent;
        this.isUserCreated = isUserCreated;
        this.fileContents = new Dictionary<string, string>();
    }

    // Add a new subdirectory
    public void AddSubDirectory(string dirName)
    {
        Directory newDir = new Directory(dirName, this);
        subDirectories.Add(newDir);
    }

    // Add a new file
    public void AddFile(string fileName)
    {
        files.Add(fileName);
    }

    // Initialize a basic file system
    public static Directory InitializeFileSystem()
    {
        // Create root directory
        Directory root = new Directory("root");

        // Create 'documents' directory with two subdirectories
        root.AddSubDirectory("documents");
        Directory documents = root.subDirectories.Find(d => d.name == "documents");
        documents.AddSubDirectory("folder1");
        documents.AddSubDirectory("folder2");

        // Create 'id' directory with a text file
        root.AddSubDirectory("id");
        Directory id = root.subDirectories.Find(d => d.name == "id");
        id.AddFile("info.txt");
        id.AddSubDirectory("folder");

        return root;
    }

    // Method to check if a file exists in the current directory
    public bool FileExists(string fileName)
    {
        // Append the file extension if it's not part of the fileName
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }

        // Check if the file is in the files list
        return files.Contains(fileName);
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
        // Append .txt if not present, this way is a user adds .txt themself it will be ignored
        if (!fileName.EndsWith(".txt"))
        {
            fileName += ".txt";
        }

        if (files.Contains(fileName))
        {
            return "File already exists: " + fileName;
        }
        else
        {
            files.Add(fileName);
            fileContents[fileName] = "testing"; // Initialize with empty content
            return "File created: " + fileName;
        }
    }

    // Method to read a file
    public string ReadFile(string fileName)
    {
        if (files.Contains(fileName))
        {
            return fileContents[fileName];
        }
        else
        {
            return "File not found: " + fileName;
        }
    }

    // Method to delete a file
    public string DeleteFile(string fileName)
    {
        // Check if the file exists
        if (files.Contains(fileName))
        {
            files.Remove(fileName);
            fileContents.Remove(fileName); // Remove file content if stored
            return "File deleted: " + fileName;
        }
        else
        {
            return "File not found: " + fileName;
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
        if (fileContents.ContainsKey(fileName))
        {
            fileContents[fileName] = content;
        }
        else
        {
            // Optionally create the file if it doesn't exist
            AddFile(fileName);
            fileContents[fileName] = content;
        }
    }

    
}
