using Godot;
using Research.Console;
using Research.Network;

namespace Research.Player;

public partial class PlayerComponent : Node
{
    [Export] public Player Parent;

    public bool IsControlled()
    {
        if (Client.Instance == null) return false;
        if (Client.Instance.ClientStatus != Client.Status.Playing) return false;
        if (Client.Instance.LocalPlayer == null) return false;
        if (ResearchConsole.Instance.ConsoleLayer.Visible) return false;
        return Client.Instance.LocalPlayer.Id == Parent.Id;
    }
}