using Godot;
using Research.Network;
using Research.Network.Packets;
using Research.Network.Packets.C2S;

namespace Research.Player;

public partial class Movement : PlayerComponent
{
    [Export] public float Speed { get; set; } = 250.0f;
    private Vector2 _currentWishDir;
    private Vector2 _lastInput;
    
    public Vector2 GetWishDir()
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        return inputDir;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Parent.Position += _currentWishDir.Normalized() * Speed * (float)delta;
        if (!IsControlled()) return;
        var wishDir = GetWishDir();
        if (_lastInput != wishDir)
        {
            var packet = new C2SMovePacket(wishDir);
            Packets.SendPacket(packet, Client.Instance);
            _lastInput = wishDir;
        }
    }

    public void SetWishDir(Vector2 wishDir)
    {
        _currentWishDir = wishDir;
    }
}