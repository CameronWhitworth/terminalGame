using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    Interpreter interpreter;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
        currentDirectory = Directory.InitializeFileSystem();
        terminalInput.ActivateInputField();
        terminalInput.Select();

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

    private void ExecuteInput(string input)
    {
        // Store command in history and reset history index to the end
        commandHistory.Add(input);
        historyIndex = commandHistory.Count;

        ClearInputField();

        AddDirectoryLine(input);

        // Add interpreter/response line
        List<string> interpretationLines = interpreter.Interpret(input);

        // Scroll to bottom
        StartCoroutine(AddLinesWithDelay(interpretationLines));
    }

    void Update()
    {
        if (terminalInput.isFocused && !isInEditorMode)
        {
            HandleCommandHistory();

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
                .Concat(currentDirectory.files)
                .Where(name => name.StartsWith(lastWord))
                .ToList();

            // If the last word is empty (i.e., the user has just typed a space), include all options
            if (string.IsNullOrEmpty(lastWord))
            {
                autocompleteOptions = autocompleteOptions.Concat(currentDirectory.subDirectories.Select(d => d.name + "/"))
                    .Concat(currentDirectory.files)
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
            AddInterpreterLines(new List<string> { "File not found: " + fileName });
            dirResponse = "File not found: " + fileName ;
            return;
        }
    }

    // Call this method when the 'save' command is entered
    public void SaveEditedFile()
    {

        currentDirectory.SaveFileContent(currentEditingFile, editorInputField.text);
        isInEditorMode = false;
        int lines = AddInterpreterLines(new List<string> { "File saved: " + currentEditingFile });

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
        int lines = AddInterpreterLines(new List<string> { "Editing cancelled." });
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

        //Set it's child index
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        //Set Text of the new game object instantiated above
        msg.GetComponentsInChildren<Text>()[1].text = userInput;
    }

    int AddInterpreterLines(List<string> interpretation)
    {
        for(int i = 0; i < interpretation.Count; i++)
        {
            //Create responce line
            GameObject res = Instantiate(responceLine, msgList.transform);

            //Set it at the end of all messages
            res.transform.SetAsLastSibling();

            //Set size of message list and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            //Set text of response line
            res.GetComponentInChildren<Text>().text = interpretation[i];
        }

        return interpretation.Count;
    }

    void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2(0, 4500);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

    IEnumerator SmoothScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // Update the canvas immediately to ensure all UI elements are current.

        // Wait for a frame to let the UI elements update their positions and sizes.
        yield return null; 

        // Ensure the scroll position is at the very bottom.
        sr.verticalNormalizedPosition = 0;
    }

    IEnumerator AddLinesWithDelay(List<string> lines)
    {
        // Disable user input at the start
        terminalInput.interactable = false;
        userInputLine.gameObject.SetActive(false);

        foreach (var line in lines)
        {
            // Create response line
            GameObject res = Instantiate(responceLine, msgList.transform);

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


}
