using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEditorManager : MonoBehaviour
{
    public InputField editorInputField; // Make sure to assign this in the Unity Editor
    public TerminalManager terminalManager; // Assign this to link with the TerminalManager

    private bool isActive = false;

    void Update()
    {
        // Only process input if the editor is active and the Return key is pressed
        if (isActive && Input.GetKeyDown(KeyCode.Return))
        {
            string input = editorInputField.text.Trim();
            if (!string.IsNullOrEmpty(input)) // Check if the input is not empty or whitespace
            {
                ExecuteEditorCommand(input);
                editorInputField.text = ""; // Clear the input field after command execution
            }
        }
    }
    private void ExecuteEditorCommand(string input)
    {
        // Split the input to get the command
        string[] args = input.Split();

        // Handle the 'save' command
        if (args[0].ToLower() == "save")
        {
            terminalManager.SaveEditedFile();
            ClearEditorInput();
        }
        // Handle the 'cancel' or 'exit' command
        else if (args[0].ToLower() == "cancel" || args[0].ToLower() == "exit")
        {
            terminalManager.CancelEditing();
            ClearEditorInput();
        }
        else
        {
            // Add error handling or other commands as necessary
            Debug.Log("Unknown editor command: " + input);
        }
    }

    // This method is called when the editor mode is entered
    public void ActivateEditor()
    {
        isActive = true;
        editorInputField.ActivateInputField();
        editorInputField.Select();
    }

    // This method is called when the editor mode is exited
    public void DeactivateEditor()
    {
        isActive = false;
        editorInputField.text = ""; // Clear the editor input field
    }

    private void ClearEditorInput()
    {
        editorInputField.text = "";
        editorInputField.ActivateInputField();
        editorInputField.Select();
    }
}
