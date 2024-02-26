using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileMetadata
{
    public string Name;
    public bool IsUserCreated;
    public string Content;

    public FileMetadata(string name, bool isUserCreated, string content = "")
    {
        Name = name;
        IsUserCreated = isUserCreated;
        Content = content;
    }
}
