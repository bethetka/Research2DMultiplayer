using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Research.Game;

namespace Research.Console;

public partial class ResearchConsole : Node
{
    private static Dictionary<string, MethodInfo> _commandMethods = new Dictionary<string, MethodInfo>();

    [Export] public static ResearchConsole Instance;

    [Export] public LineEdit Command;
    [Export] public CanvasLayer ConsoleLayer;
    [Export] public CanvasLayer BackdropLayer;
    [Export] public RichTextLabel Content;

    static ResearchConsole()
    {
        RegisterCommands();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;

        Command.TextSubmitted += text =>
        {
            Command.Text = "";
            Content.AddText("> ");
            Content.AddText(text);
            Content.AddText("\n");
            Parse(text);
            Content.ScrollToLine(Content.GetLineCount());
        };
    }

    private static void RegisterCommands()
    {
        var methods = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.GetCustomAttributes(typeof(ConsoleCommandAttribute), false).Length > 0)
            .ToArray();

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ConsoleCommandAttribute>();
            _commandMethods[attr.Name.ToLower()] = method;
        }
    }

    public void Parse(string input)
    {
        var parts = SplitCommand(input);
        if (parts.Length == 0) return;

        string commandName = parts[0].ToLower();
        if (_commandMethods.TryGetValue(commandName, out MethodInfo method))
        {
            try
            {
                var parameters = method.GetParameters();
                var args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length && i + 1 < parts.Length; i++)
                {
                    args[i] = Convert.ChangeType(parts[i + 1], parameters[i].ParameterType);
                }


                method.Invoke(null, args);
            }
            catch (Exception e)
            {
                Err("When executing command, exception ocurred:");
                Err(e.Message);
                Err("Stacktrace:");
                Err(e.StackTrace ?? "[empty]");
            }
        }
        else
        {
            Err($"Unknown command: {commandName}");
        }
        
    }

    private string[] SplitCommand(string input)
    {
        var parts = new List<string>();
        var currentPart = "";
        var inQuotes = false;

        foreach (char c in input)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                if (!inQuotes && currentPart.Length > 0)
                {
                    parts.Add(currentPart);
                    currentPart = "";
                }
            }
            else if (c == ' ' && !inQuotes)
            {
                if (currentPart.Length > 0)
                {
                    parts.Add(currentPart);
                    currentPart = "";
                }
            }
            else
            {
                currentPart += c;
            }
        }

        if (currentPart.Length > 0)
        {
            parts.Add(currentPart);
        }

        return parts.ToArray();
    }

    private void Log(string color, string prefix, string data)
    {
        string bbcode = $"[color={color}][{prefix}] {data}[/color]";
        GD.PrintRich(bbcode);
        if (!GameArguments.IsServer())
        {
            Content.AppendText(bbcode + "\n");
        }
    }
    
    public void Info(params string[] args)
    {
        Log("white", "INFO", string.Join(" ", args));
    }

    public void Warn(params string[] args)
    {
        Log("yellow", "WARNING", string.Join(" ", args));
    }

    public void Err(params string[] args)
    {
        Log("red", "ERROR", string.Join(" ", args));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        BackdropLayer.Visible = ConsoleLayer.Visible;
        if (Input.IsActionJustPressed("toggle_console"))
        {
            ConsoleLayer.Visible = !ConsoleLayer.Visible;
            if (ConsoleLayer.Visible)
            {
                Command.GrabFocus();
                Command.Text = "";
            }
        }
    }
}