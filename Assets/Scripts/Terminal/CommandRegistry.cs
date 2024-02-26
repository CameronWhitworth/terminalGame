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
        commands.Add("ascii", new AsciiCommand());
        commands.Add("ls", new LsCommand());
        commands.Add("color", new SwitchThemeCommand());
        commands.Add("alias", new AliasCommand());
        commands.Add("echo", new EchoCommand());
        commands.Add("cd", new CdCommand());
        commands.Add("mkdir", new MkdirCommand());
        commands.Add("rmdir", new RmdirCommand());
        commands.Add("rm", new RmCommand());
        commands.Add("delete", new RmCommand());
        commands.Add("remove", new RmCommand());
        commands.Add("open", new OpenCommand());
        commands.Add("cat", new OpenCommand());
        commands.Add("touch", new TouchCommand());
        commands.Add("edit", new EditCommand());
        commands.Add("history", new HistoryCommand());
        commands.Add("tree", new TreeCommand());
        commands.Add("sys", new SysInfoCommand());
        commands.Add("sysinfo", new SysInfoCommand());
        commands.Add("system", new SysInfoCommand());
        commands.Add("man", new ManCommand());
    }

    public ICommand GetCommand(string commandName)
    {
        if (commands.TryGetValue(commandName, out ICommand command))
        {
            return command;
        }
        return null;
    }
}
