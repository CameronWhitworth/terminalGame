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
        commands.Add("pass", new PasswordCommand());
        commands.Add("password", new PasswordCommand());
        commands.Add("whereami", new PwdCommand());
        commands.Add("pwd", new PwdCommand());
        commands.Add("calc", new CalcCommand());
        commands.Add("cp", new FileMovementCommand());
        commands.Add("copy", new FileMovementCommand());
        commands.Add("mv", new FileMovementCommand());
        commands.Add("move", new FileMovementCommand());
        commands.Add("grep", new GrepCommand());
        commands.Add("sort", new SortCommand());
        commands.Add("uniq", new UniqCommand());
        commands.Add("diff", new DiffCommand());
        commands.Add("wc", new WcCommand());
        commands.Add("clear", new ClearCommand());
        commands.Add("boobies", new BoobCommand());
    }

    public ICommand GetCommand(string commandName)
    {
        if (commands.TryGetValue(commandName, out ICommand command))
        {
            return command;
        }
        return null;
    }

    public IEnumerable<string> GetAllCommands()
    {
        return commands.Keys;
    }
}
