using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Research.Game;

public class GameArguments
{
    public class Argument
    {
        public bool IsFlag;
        public string Name;
        public string Value;

        public override string ToString()
        {
            return $"Argument{{IsFlag={IsFlag}, Name={Name}, Value={Value}}}";
        }
    }

    public static Argument[] ParseArguments()
    {
        List<Argument> arguments = new List<Argument>();
        var args = OS.GetCmdlineArgs();
        foreach (var arg in args)
        {
            if (!arg.StartsWith("--")) continue;
            string[] parts = arg.Split("=");
            string name = parts[0].Substring(2);
            if (parts.Length >= 2)
            {
                arguments.Add(new Argument
                {
                    IsFlag = false,
                    Name = name,
                    Value = string.Join("=", parts.Skip(1))
                });
            }
            else
            {
                arguments.Add(new Argument
                {
                    IsFlag = true,
                    Name = name,
                    Value = null
                });
            }
        }

        return arguments.ToArray();
    }

    public static bool HasFlag(string flag)
    {
        var args = ParseArguments();

        return args.FirstOrDefault(i => i.IsFlag && i.Name.Equals(flag)) == null;
    }
    
    public static string GetArgument(string flag)
    {
        var args = ParseArguments();
        var arg = args.FirstOrDefault(i => !i.IsFlag && i.Name.Equals(flag));
        return arg?.Value;
    }

    public static bool IsServer()
    {
        return OS.HasFeature("dedicated_server") || DisplayServer.GetName() == "headless";
    }
}