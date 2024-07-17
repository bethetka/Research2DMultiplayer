using System;
using Godot;
using Research.Console;
using Research.Network;

namespace Research.Game;

public partial class Game : Node
{
    public override void _Ready()
    {
        var xStr = GameArguments.GetArgument("x");
        var yStr = GameArguments.GetArgument("y");
        var wStr = GameArguments.GetArgument("w");
        var hStr = GameArguments.GetArgument("h");
        var hostStr = GameArguments.GetArgument("connect-host");
        var portStr = GameArguments.GetArgument("connect-port");
        var win = GetTree().Root;
        
        if (wStr != null && hStr != null)
        {
            ResearchConsole.Instance.Info($"Resizing window to {wStr},{hStr}");
            int width = Convert.ToInt32(wStr);
            int height = Convert.ToInt32(hStr);
            DisplayServer.WindowSetSize(new Vector2I(width, height));
        }

        if (xStr != null && yStr != null)
        {
            ResearchConsole.Instance.Info($"Moving window to {xStr},{yStr}");
            float dpiScale = DisplayServer.ScreenGetScale();
            int x = Convert.ToInt32(xStr) * (int)dpiScale;
            int y = Convert.ToInt32(yStr) * (int)dpiScale;
            win.Position = new Vector2I(x, y);
        }

        if (hostStr != null && portStr != null)
        {
            ushort port = Convert.ToUInt16(portStr);
            
            Client.Connect(hostStr, port);
        }
    }
}