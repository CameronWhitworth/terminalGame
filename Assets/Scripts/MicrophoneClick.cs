using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MicrophoneClick : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public string startNode = "Start"; // Replace with your start node name if different

    void OnMouseDown()
    {
        // Check if the dialogue is already running to avoid restarting it
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(startNode);
        }
    }
}