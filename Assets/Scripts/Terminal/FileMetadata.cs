using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FileMetadata
{
    public string Name;
    public bool IsUserCreated;
    public string Content;
    public bool IsPasswordProtected => !string.IsNullOrEmpty(Password);
    public string Password; // The password for the file

    public FileMetadata(string name, bool isUserCreated, string content = "",  string password = "")
    {
        Name = name;
        IsUserCreated = isUserCreated;
        Content = content;
        Password = password;
    }

    // Add or update password
    public void SetPassword(string newPassword)
    {
        Password = newPassword;
    }

    // Remove password
    public void RemovePassword()
    {
        Password = null;
    }

}
