using Godot;

namespace Research.Console.commands;

public class DebugCommands
{
    [ConsoleCommand("info")]
    public static void InfoCommand()
    {
        ResearchConsole.Instance.Info("ProjectResearch indev");
        ResearchConsole.Instance.Info("Author: nosqd, bethetka studios");
        ResearchConsole.Instance.Info("Join our discord server: https://discord.gg/VFJbrCCkKd");
    }
    

    [ConsoleCommand("quit")]
    public static void QuitCommand()
    {
        ResearchConsole.Instance.GetTree().Quit();
    }
    
    [ConsoleCommand("clear")]
    public static void ClearCommand()
    {
        ResearchConsole.Instance.Content.Clear();
    }
}