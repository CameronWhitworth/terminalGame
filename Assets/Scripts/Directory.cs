using System.Collections.Generic;

[System.Serializable]
public class Directory
{
    public string name;
    public List<Directory> subDirectories;
    public List<string> files;
    public Directory parent;
    public bool isUserCreated;

    public Directory(string name, Directory parent = null, bool isUserCreated = false)
    {
        this.name = name;
        this.subDirectories = new List<Directory>();
        this.files = new List<string>();
        this.parent = parent;
        this.isUserCreated = isUserCreated;
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

    // Additional methods for directory management...
}
