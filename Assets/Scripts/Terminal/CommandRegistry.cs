using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRegistry
{
    private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

    public CommandRegistry()
    {
        // Register all commands
        commands.Add("help", new HelpCommand());
        commands.Add("ls", new LsCommand());
        commands.Add("color", new SwitchThemeCommand());
        // Add other commands here
    }

    public ICommand GetCommand(string commandName)
    {
        if (commands.TryGetValue(commandName, out ICommand command))
        {
            return command;
        }

        return null; // Or a default command that indicates command not found
    }
}
