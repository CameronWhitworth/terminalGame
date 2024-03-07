using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class TerminalManager : MonoBehaviour, IPointerClickHandler
{
    public GameObject directoryLine;
    public GameObject responceLine;
    public InputField terminalInput;
    public GameObject userInputLine;
    public GameObject editorUI;
    public GameObject commandLineUI;
    public ScrollRect sr;
    public GameObject msgList;
    private Directory currentDirectory;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = 0;
    private int autocompleteIndex = -1;
    private List<string> autocompleteOptions = new List<string>();
    private bool isInEditorMode = false;
    private string currentEditingFile = "";
    public InputField editorInputField;
    public TextEditorManager textEditorManager;
    private ThemeManager themeManager;
    private CommandRegistry commandRegistry;
    private bool isAwaitingPassword = false;
    private string awaitingPasswordForFile = "";
    private Action<bool> onPasswordEnteredCallback;
    public delegate void PasswordInputCallback(bool isCorrect);

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
        currentDirectory = Directory.InitializeFileSystem();
        commandRegistry = new CommandRegistry();
        terminalInput.ActivateInputField();
        terminalInput.Select();
        InitializeThemeManager();

        // Deactivate editor UI and input field
        editorUI.SetActive(false);
        textEditorManager.editorInputField.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        terminalInput.ActivateInputField();
        terminalInput.Select();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteInput(terminalInput.text);
        }
        else if (terminalInput.isFocused && terminalInput.text == "" && Input.GetKeyDown(KeyCode.Return))
        {
            ClearInputField();
            AddDirectoryLine("");
            List<string> list = new List<string>();
            StartCoroutine(AddLinesWithDelay(list));

        }
    }



    // This method is invoked when a password input is required.
    public void EnablePasswordInputMode(string fileName, Action<bool> callback)
    {
        isAwaitingPassword = true;
        awaitingPasswordForFile = fileName;
        onPasswordEnteredCallback = callback;

        // Prompt user to enter password (you could update UI or print a message)
       StartCoroutine(AddLinesWithDelay(new List<string> {"Password required to access file. Please enter password:"}));
    }
    
    private void ExecuteInput(string input)
    {


        if (isAwaitingPassword)
        {
            // Handle password input
            ProcessPasswordInput(input);
        }
        else
        {
            // Existing history management and input field clearing logic...
            commandHistory.Add(input);
            if (commandHistory.Count > 250)
            {
                commandHistory.RemoveAt(0); // Maintain history size
            }
            historyIndex = commandHistory.Count;
            ClearInputField();
            AddDirectoryLine(input); // Adds the user input as a directory line

            // Interpret the command and get the response lines
            List<string> interpretationLines = interpreter.Interpret(input);
            List<string> processedLines = new List<string>();

            // Process each interpretation line to handle long responses
            foreach (var line in interpretationLines)
            {
                // Split long lines and add them to the processedLines list
                processedLines.AddRange(SplitIntoLines(line, CalculateMaxChars()));
            }

            // Use AddLinesWithDelay to display all processed lines
            StartCoroutine(AddLinesWithDelay(processedLines));
        }
    }



    private void ProcessPasswordInput(string input)
    {
        var fileMetadata = currentDirectory.GetFileMetadata(awaitingPasswordForFile);
        if (fileMetadata != null && input == fileMetadata.Password)
        {
            onPasswordEnteredCallback?.Invoke(true);
        }
        else
        {
            onPasswordEnteredCallback?.Invoke(false);
        }

        // Reset password mode
        isAwaitingPassword = false;
        awaitingPasswordForFile = "";
        onPasswordEnteredCallback = null;

        // Clear input field to ready for next command
        ClearInputField();
    }

    public void RequestPasswordInput(string fileName, Action<bool> onPasswordVerified) {
        var fileMetadata = GetCurrentDirectory().GetFileMetadata(fileName);
        if (fileMetadata == null || !fileMetadata.IsPasswordProtected) {
            onPasswordVerified?.Invoke(false);
            return;
        }

        EnablePasswordInputMode(fileName, isCorrect => {
            onPasswordVerified.Invoke(isCorrect);
        });
    }

    private List<string> SplitIntoLines(string response, int maxCharsPerLine)
    {
        List<string> lines = new List<string>();
        string currentLine = "";
        int currentLineCharCount = 0; // Counts characters excluding rich text tags
        int currentIndex = 0;
        
        while (currentIndex < response.Length)
        {
            // Check for a rich text tag start
            if (response[currentIndex] == '<')
            {
                // Find the end of the rich text tag
                int tagCloseIndex = response.IndexOf('>', currentIndex);
                if (tagCloseIndex != -1)
                {
                    // Add the entire tag to the current line without affecting the character count
                    string tag = response.Substring(currentIndex, tagCloseIndex - currentIndex + 1);
                    currentLine += tag;
                    currentIndex = tagCloseIndex + 1; // Move past the tag

                    // Check if it's a closing tag, and if so, reset formatting if needed
                    if (tag.StartsWith("</"))
                    {
                        // Logic to handle resetting formatting if necessary
                    }

                    continue; // Move to the next character/tag
                }
            }

            // Add the character to the current line and increment counters
            currentLine += response[currentIndex];
            currentLineCharCount++;
            currentIndex++;

            // If the current line reaches the max character count or the end of the response
            if (currentLineCharCount >= maxCharsPerLine || currentIndex >= response.Length)
            {
                lines.Add(currentLine); // Add the current line to the list
                currentLine = ""; // Reset for the next line
                currentLineCharCount = 0;
            }
        }

        // Add any remaining text to the last line
        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        return lines;
    }


    private int CalculateMaxChars()
    {
        // If textsize changing is added this function cant be static
        // Return the estimated max number of characters per line
        // This will need adjustment based on your UI setup
        return 80; // Example value, adjust as necessary
    }

    void Update()
    {
        if (terminalInput.isFocused && !isInEditorMode)
        {

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || 
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                HandleScrolling();
            }
            else 
            {
                HandleCommandHistory();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                AutocompleteInput(terminalInput.text);
            }

            // If user starts typing, clear the autocomplete options
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Tab) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
            {
                autocompleteOptions.Clear();
                autocompleteIndex = -1;
            }
        }
        
    }

    private void HandleCommandHistory()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            historyIndex = Mathf.Clamp(historyIndex - 1, 0, commandHistory.Count);
            terminalInput.text = historyIndex < commandHistory.Count ? commandHistory[historyIndex] : "";
            terminalInput.MoveTextEnd(false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            historyIndex = Mathf.Clamp(historyIndex + 1, 0, commandHistory.Count);
            terminalInput.text = historyIndex < commandHistory.Count ? commandHistory[historyIndex] : "";
            terminalInput.MoveTextEnd(false);
        }
    }

    private void AutocompleteInput(string input)
    {
        // Split the input into parts to identify the last word for completion
        var parts = input.Split(' ');
        var lastWord = parts.LastOrDefault();
        var basePath = string.Join(" ", parts.Take(parts.Length - 1));

        // Check if the last word has changed since the last autocomplete operation
        if (autocompleteIndex < 0 || !autocompleteOptions[autocompleteIndex].EndsWith(lastWord))
        {
            // Populate or repopulate the autocomplete options based on the last word
            autocompleteOptions = currentDirectory.subDirectories.Select(d => d.name + "/")
                .Concat(currentDirectory.files.Select(f => f.Name))
                .Where(name => name.StartsWith(lastWord))
                .ToList();

            // If the last word is empty (i.e., the user has just typed a space), include all options
            if (string.IsNullOrEmpty(lastWord))
            {
                autocompleteOptions = autocompleteOptions.Concat(currentDirectory.subDirectories.Select(d => d.name + "/"))
                    .Concat(currentDirectory.files.Select(f => f.Name))
                    .ToList();
            }

            // Reset the autocomplete index
            autocompleteIndex = -1;
        }

        // Cycle through the autocomplete options if any are available
        if (autocompleteOptions.Count > 0)
        {
            autocompleteIndex = (autocompleteIndex + 1) % autocompleteOptions.Count;
            // Append the last part to the base path only if there is a base path
            terminalInput.text = (basePath.Length > 0 ? basePath + " " : "") + autocompleteOptions[autocompleteIndex];
            terminalInput.caretPosition = terminalInput.text.Length; // Move the caret to the end
        }
    }

    private void HandleScrolling()
    {
        // Check if either Shift or Control is being held down
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || 
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Scroll up the terminal output
                ScrollTerminal(1); // Scroll up
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Scroll down the terminal output
                ScrollTerminal(-1); // Scroll down
            }
        }
    }

    public void EditFile(string fileName, out string dirResponse)
    {
        dirResponse = "";
        // Check if the file exists before entering editing mode
        if (currentDirectory.FileExists(fileName) == true)
        {
            isInEditorMode = true;
            editorUI.SetActive(true);
            commandLineUI.SetActive(false); 
            string fileContent = currentDirectory.ReadFile(fileName);
            // Load content into a UI element for editing
            editorInputField.text = fileContent;
            currentEditingFile = fileName;
            // Activate the editor input field
            textEditorManager.editorInputField.gameObject.SetActive(true);
            textEditorManager.ActivateEditor();
            editorInputField.ActivateInputField(); // Set focus to the editor input field
            editorInputField.Select();
        }
        else
        {
            StartCoroutine(AddLinesWithDelay(new List<string> { "File not found: " + currentEditingFile }));
            dirResponse = "File not found: " + fileName ;
            return;
        }
    }

    // Call this method when the 'save' command is entered
    public void SaveEditedFile()
    {
        currentDirectory.SaveFileContent(currentEditingFile, editorInputField.text);
        isInEditorMode = false;

        // Start the coroutine without trying to capture its return value
        StartCoroutine(AddLinesWithDelay(new List<string> { "File saved: " + currentEditingFile }));

        // Deactivate the editor input field
        textEditorManager.editorInputField.gameObject.SetActive(false);
        textEditorManager.DeactivateEditor();

        commandLineUI.SetActive(true);
        editorUI.SetActive(false);
        terminalInput.ActivateInputField(); // Set focus back to the terminal input field
        terminalInput.Select();
        userInputLine.transform.SetAsLastSibling();

        // Scroll to bottom
        StartCoroutine(SmoothScrollToBottom());
    }

    // Call this method when the 'cancel' command is entered
    public void CancelEditing()
    {
        isInEditorMode = false;
        editorUI.SetActive(false); // Hide the editor UI
        commandLineUI.SetActive(true); // Re-enable the command line UI
        StartCoroutine(AddLinesWithDelay(new List<string> { "Canceled editing: " + currentEditingFile }));
        // Deactivate the editor input field
        textEditorManager.editorInputField.gameObject.SetActive(false);
        textEditorManager.DeactivateEditor();
        editorUI.SetActive(false);
        terminalInput.ActivateInputField(); // Set focus back to the terminal input field
        terminalInput.Select();
        userInputLine.transform.SetAsLastSibling();
        // Scroll to bottom
        StartCoroutine(SmoothScrollToBottom());
    }


    void ClearInputField()
    {
        terminalInput.text = "";
        autocompleteOptions.Clear();
    }

    public void ClearScreen()
    {
        // Calculate the total height of existing lines
        float totalHeight = 0f;
        foreach (Transform child in msgList.transform)
        {
            totalHeight += child.GetComponent<RectTransform>().rect.height;
        }

        // Reset the position of the existing userInputLine below the existing lines
        float newLineHeight = userInputLine.GetComponent<RectTransform>().rect.height;
        float newYPosition = -totalHeight - newLineHeight;
        userInputLine.transform.localPosition = new Vector3(0f, newYPosition, 0f);

        // Destroy all child objects of msgList except the userInputLine
        foreach (Transform child in msgList.transform)
        {
            if (child.gameObject != userInputLine)
            {
                Destroy(child.gameObject);
            }
        }

        // Reset the size of msgList
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

        // Ensure that the ScrollRect content is still set
        ScrollRect scrollRect = sr.GetComponent<ScrollRect>();
        if (scrollRect.content == null)
        {
            scrollRect.content = msgList.GetComponent<RectTransform>();
        }

        // Reset the vertical normalized position to the bottom
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void AddDirectoryLine(string userInput)
    {
        //Resize command line container, so scrollrect doesn't scream
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        //Instantiate the dir line
        GameObject msg = Instantiate(directoryLine, msgList.transform);

        Text textComponent = msg.GetComponentInChildren<Text>();
        if (textComponent != null)
        {
            themeManager.ApplyColorToText(textComponent); // Update this line's color
        }

        //Set it's child index
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        //Set Text of the new game object instantiated above
        msg.GetComponentsInChildren<Text>()[1].text = userInput;
    }

    IEnumerator SmoothScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // Update the canvas immediately to ensure all UI elements are current.

        // Wait for a frame to let the UI elements update their positions and sizes.
        yield return null; 

        // Ensure the scroll position is at the very bottom.
        sr.verticalNormalizedPosition = 0;
    }

    private void ScrollTerminal(int direction)
    {
        // This the current height the gameobject. one we make is a verible we can scale remember to adjust this to keep scroll by one line consitant
        float singleLineHeight = 35.0f; 

        // Calculate the total content height
        float totalContentHeight = msgList.GetComponent<RectTransform>().sizeDelta.y;

        // Calculate the visible height of the ScrollRect's viewport
        float viewportHeight = sr.viewport.rect.height;

        // Calculate the total number of visible lines in the viewport
        int totalVisibleLines = Mathf.FloorToInt(viewportHeight / singleLineHeight);

        // Calculate the scroll step as a proportion of one line per the total number of lines
        float scrollStep = 1f / (totalContentHeight / singleLineHeight - totalVisibleLines + 1);

        // Adjust the verticalNormalizedPosition based on the scroll direction and calculated step
        sr.verticalNormalizedPosition = Mathf.Clamp01(sr.verticalNormalizedPosition + scrollStep * direction);
    }

    public IEnumerator AddLinesWithDelay(List<string> lines)
    {
        // Disable user input at the start
        terminalInput.interactable = false;
        userInputLine.gameObject.SetActive(false);

        foreach (var line in lines)
        {
            // Create response line
            GameObject res = Instantiate(responceLine, msgList.transform);

            // Apply theme to the new text element
            Text textComponent = res.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                themeManager.ApplyColorToText(textComponent); // Update this line's color
            }

            // Set it at the end of all messages
            res.transform.SetAsLastSibling();

            // Set size of message list and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            // Set text of response line
            res.GetComponentInChildren<Text>().text = line;

            // Scroll to the newly added line smoothly
            StartCoroutine(SmoothScrollToBottom());

            // Wait for a specified delay before adding the next line
            yield return new WaitForSeconds(0.03f); // Adjust this delay to match the desired speed

            userInputLine.transform.SetAsLastSibling();
        }
        
        // After adding all lines, move the user input line to the end
        userInputLine.transform.SetAsLastSibling();
        userInputLine.gameObject.SetActive(true);

        // Re-enable user input at the end
        terminalInput.interactable = true;

        // Optionally, ensure the scroll view is at the bottom after all lines have been added and user input is re-enabled
        StartCoroutine(SmoothScrollToBottom());

        // Refocus on the input field after re-enabling it
        terminalInput.ActivateInputField();
        terminalInput.Select();
    }


    public void ChangeDirectory(string directoryName, out string dirResponse)
    {
        dirResponse = "";

        // Check if the input is 'cd /' to go back to the root
        if (directoryName == "root")
        {
            // Set currentDirectory to the root
            currentDirectory = FindRootDirectory();
            dirResponse = "Changed directory to root";
        }
        else if (directoryName == "..")
        {
            if (currentDirectory.parent != null)
            {
                // Go back to the parent directory
                currentDirectory = currentDirectory.parent;
                dirResponse = "Changed directory to " + currentDirectory.name;
            }
            else
            {
                dirResponse = "Current directory has no parent";
            }
        }
        else if (directoryName == "/")
        {
            while (currentDirectory.parent != null)
            {
                currentDirectory = currentDirectory.parent;
                dirResponse = "Changed directory to " + currentDirectory.name;
            }
        }
        else 
        {
            // Search for the directory in the current directory's subdirectories
            bool directoryFound = false;
            foreach (var dir in currentDirectory.subDirectories) 
            {
                if (dir.name == directoryName) 
                {
                    currentDirectory = dir;
                    dirResponse = "Changed directory to " + dir.name;
                    directoryFound = true;
                    break;
                }
            }

            if (!directoryFound)
            {
                dirResponse = "Directory not found: " + directoryName;
            }
        }
    }

    // Method to find the root directory from any current directory
    private Directory FindRootDirectory()
    {
        Directory dir = currentDirectory;
        while (dir.parent != null)
        {
            dir = dir.parent;
        }
        return dir;
    }

    
    // Method to get the current directory
    public Directory GetCurrentDirectory()
    {
        return currentDirectory;
    }

    //TODO: Finsih this shit, it doesn't work but I dont care it's just a silly little option
    public void ChangeTextSize(int newSize)
    {
        foreach (Transform child in msgList.transform)
        {
            Text textComponent = child.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.fontSize = newSize;
            }
        }

        // Force the canvas to update
        Canvas.ForceUpdateCanvases();
    }

    public IReadOnlyList<string> GetCommandHistory()
    {
        return commandHistory.AsReadOnly();
    }

    private void InitializeThemeManager()
    {
        themeManager = new ThemeManager();
        // Add more themes or configure them here if needed
    }

    public ThemeManager GetThemeManager()
    {
        return themeManager;
    }

    public CommandRegistry GetCommandRegistry()
    {
        return commandRegistry;
    }

    private void ChangeTheme(string themeName)
    {
        // Just call SetTheme with the theme name
        bool themeSet = themeManager.SetTheme(themeName);
        
        // Handle the result of the theme change
        if (!themeSet)
        {
            Debug.LogError("Theme change failed: Theme not found.");
        }
    }
}
