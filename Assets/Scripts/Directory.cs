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

    
}
