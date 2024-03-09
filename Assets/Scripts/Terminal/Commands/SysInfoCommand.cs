using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SysInfoCommand : ICommand
{
    public List<string> Execute(string[] args, TerminalManager terminalManager, List<string> previousOutput = null)
    {
        List<string> response = new List<string>();

        // Mock vintage system information, update this with more logical info
        response.Add("System Information (Mock Vintage Data)");
        response.Add("------------------------------------");
        response.Add("Processor: Zilog Z80 @ 2.5 MHz");
        response.Add("Memory: 64 KB");
        response.Add("Storage: Dual 5.25\" Floppy Drive (360 KB each)");
        response.Add("Display: Monochrome CRT, 80x25 Text Mode");
        response.Add("Operating System: CP/M 2.2");
        response.Add("Network: None");
        response.Add("System Time: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        response.Add("User Logged: admin");
        response.Add("------------------------------------");
        response.Add("Note: This system does not support multitasking.");

        return response;
    }
}