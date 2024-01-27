using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responceLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;
    private Directory currentDirectory;
    private List<string> commandHistory = new List<string>();
    private int historyIndex = 0;



    Interpreter interpreter;
    private void Start()
    {
        interpreter = GetComponent<Interpreter>();

        // Correctly build file structure
        currentDirectory = Directory.InitializeFileSystem();

        terminalInput.ActivateInputField();
        terminalInput.Select();
    }


    private void OnGUI()
    {
        if(terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {

            // Store command in history and reset history index to the end
            commandHistory.Add(terminalInput.text);
            historyIndex = commandHistory.Count;

            //Store user input
            string userInput = terminalInput.text;

            ClearInputFeild();

            AddDirectoryLine(userInput);

            //Add interpreter/response line
            int lines = AddInterpreterLines(interpreter.Interpret(userInput));

            //Scroll to bottom
            ScrollToBottom(lines);

            //Move user input to the end
            userInputLine.transform.SetAsLastSibling();

            //Refucus the input feild
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    void Update()
    {
        if (terminalInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    terminalInput.text = commandHistory[historyIndex];
                    terminalInput.MoveTextEnd(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    terminalInput.text = commandHistory[historyIndex];
                    terminalInput.MoveTextEnd(false);
                }
                else if (historyIndex == commandHistory.Count - 1)
                {
                    historyIndex++;
                    terminalInput.text = ""; // Clear input field at the end of history
                }
            }
        }
    }

    void ClearInputFeild()
    {
        terminalInput.text = "";
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

    public void ChangeDirectory(string directoryName, out string dirResponse)
    {
        dirResponse = "";
        if (directoryName == ".." && currentDirectory.parent != null) 
        {
            // Go back to the parent directory
            currentDirectory = currentDirectory.parent;
            dirResponse = "Changed directory to " + currentDirectory; //TODO: Make display correct name
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

            // Handle directory not found
            if (!directoryFound)
            {
                dirResponse = "Directory not found: " + directoryName;
            }
        }
    }
    
    // Method to get the current directory
    public Directory GetCurrentDirectory()
    {
        return currentDirectory;
    }
}
